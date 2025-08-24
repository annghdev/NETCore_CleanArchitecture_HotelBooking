using HotelBooking.Application.Common;
using HotelBooking.Application.ReadModels;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Calendar.Queries.GetBookingCalendar;

public class GetBookingCalendarQueryHandler : IRequestHandler<GetBookingCalendarQuery, CalendarMatrix>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetBookingCalendarQueryHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<CalendarMatrix> Handle(GetBookingCalendarQuery request, CancellationToken cancellationToken)
    {
        // Normalize timezone inputs
        var normalizedRequest = request.NormalizeVietnamTimezone();
        
        // Ensure input date có proper Vietnam timezone
        var vietnamDate = TimeZoneHelper.EnsureVietnamTimeZone(normalizedRequest.Date);
        
        // Tạo time frames cho ngày (theo giờ Vietnam)
        var timeFrames = GenerateTimeFrames();
        
        // Lấy tất cả phòng active
        var rooms = await _roomRepository.GetQueryable()
            .Where(r => r.IsActive)
            .OrderBy(r => r.Floor)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);

        // Convert Vietnam date thành UTC range để query database
        var dayStartUtc = TimeZoneHelper.GetVietnamDayStartUtc(vietnamDate);
        var dayEndUtc = TimeZoneHelper.GetVietnamDayEndUtc(vietnamDate);

        var bookings = await _bookingRepository.GetQueryable()
            .Where(b => 
                // Booking active
                (b.Status == BookingStatus.Confirmed ||
                 b.Status == BookingStatus.CheckedIn ||
                 b.Status == BookingStatus.Pending) &&
                // Overlap với ngày được chọn (so sánh với UTC times trong database)
                b.CheckInDateTime < dayEndUtc &&
                b.CheckOutDateTime > dayStartUtc)
            .Include(b => b.Rooms)
            .Include(b => b.Customer)
            .ToListAsync(cancellationToken);

        // Tạo rows cho calendar
        var rows = new List<CalendarRow>();

        foreach (var room in rooms)
        {
            var cells = CreateMergedCellsForRoom(room, timeFrames, bookings, vietnamDate);

            rows.Add(new CalendarRow
            {
                RoomId = room.Id,
                RoomName = room.Name,
                Cells = cells
            });
        }

        return new CalendarMatrix
        {
            DisplayDate = vietnamDate.Date,
            Columns = timeFrames,
            Rows = rows
        };
    }

    /// <summary>
    /// Tạo cells cho một phòng với logic merge cells cùng bookingId
    /// </summary>
    private static List<CalendarCell> CreateMergedCellsForRoom(
        Domain.Entities.Room room, 
        List<TimeFrame> timeFrames, 
        List<Booking> bookings, 
        DateTimeOffset vietnamDate)
    {
        var cells = new List<CalendarCell>();
        var processedTimeFrames = new HashSet<int>();

        foreach (var timeFrame in timeFrames)
        {
            if (processedTimeFrames.Contains(timeFrame.Id))
                continue;

            // Tạo time frame theo Vietnam timezone
            var vietnamDayStart = new DateTimeOffset(vietnamDate.Date, TimeZoneHelper.VietnamOffset);
            var timeFrameStartVN = vietnamDayStart.AddHours(timeFrame.StartTime.Hour).AddMinutes(timeFrame.StartTime.Minute);
            var timeFrameEndVN = vietnamDayStart.AddHours(timeFrame.EndTime.Hour).AddMinutes(timeFrame.EndTime.Minute);

            // Tìm booking cho phòng này trong time frame này
            var roomBooking = bookings.FirstOrDefault(b => 
                b.Rooms!.Any(br => br.RoomId == room.Id) &&
                IsBookingInTimeFrame(b, timeFrameStartVN, timeFrameEndVN));

            if (roomBooking == null)
            {
                // Không có booking, tạo cell thường
                cells.Add(CreateCalendarCell(timeFrame.Id, null));
                processedTimeFrames.Add(timeFrame.Id);
            }
            else
            {
                // Có booking, tính toán colSpan để merge cells
                var colSpan = CalculateColSpanForBooking(roomBooking, timeFrame, timeFrames, vietnamDate, room.Id);
                
                // Tạo cell đầu tiên với colSpan
                var mergedCell = CreateCalendarCell(timeFrame.Id, roomBooking);
                mergedCell.ColSpan = colSpan;
                cells.Add(mergedCell);

                // Đánh dấu các time frames đã được process
                for (int i = 0; i < colSpan; i++)
                {
                    var frameIndex = timeFrame.Id + i;
                    if (frameIndex < 24)
                        processedTimeFrames.Add(frameIndex);
                }
            }
        }

        return cells;
    }

    /// <summary>
    /// Tính toán ColSpan cho một booking
    /// </summary>
    private static int CalculateColSpanForBooking(
        Booking booking, 
        TimeFrame startTimeFrame, 
        List<TimeFrame> allTimeFrames, 
        DateTimeOffset vietnamDate,
        int roomId)
    {
        int colSpan = 1;
        
        // Lấy thời gian effective của booking và convert sang Vietnam timezone
        DateTimeOffset effectiveCheckIn;
        DateTimeOffset effectiveCheckOut;

        switch (booking.Status)
        {
            case BookingStatus.CheckedIn:
                var checkedInTime = booking.CheckedInAt ?? booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkedInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime);
                break;
            case BookingStatus.CheckedOut:
                return 0; // Không hiển thị
            default:
                var checkInTime = booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime2 = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime2);
                break;
        }

        // Đếm số time frames liên tiếp mà booking này chiếm
        var vietnamDayStart = new DateTimeOffset(vietnamDate.Date, TimeZoneHelper.VietnamOffset);
        
        for (int i = startTimeFrame.Id + 1; i < allTimeFrames.Count; i++)
        {
            var nextFrame = allTimeFrames[i];
            var nextFrameStart = vietnamDayStart.AddHours(nextFrame.StartTime.Hour).AddMinutes(nextFrame.StartTime.Minute);
            var nextFrameEnd = vietnamDayStart.AddHours(nextFrame.EndTime.Hour).AddMinutes(nextFrame.EndTime.Minute);

            // Kiểm tra booking có overlap với frame tiếp theo không
            if (effectiveCheckIn < nextFrameEnd && effectiveCheckOut > nextFrameStart)
            {
                colSpan++;
            }
            else
            {
                break;
            }
        }

        return colSpan;
    }

    /// <summary>
    /// Tạo time frames cho ngày (24 giờ)
    /// </summary>
    private static List<TimeFrame> GenerateTimeFrames()
    {
        var timeFrames = new List<TimeFrame>();
        
        for (int hour = 0; hour < 24; hour++)
        {
            timeFrames.Add(new TimeFrame
            {
                Id = hour,
                StartTime = new TimeOnly(hour, 0),
                EndTime = new TimeOnly(hour == 23 ? 23 : hour + 1, hour == 23 ? 59 : 0),
                BasePrice = hour >= 6 && hour <= 22 ? 100000 : 80000 // Giá theo giờ vàng
            });
        }

        return timeFrames;
    }

    /// <summary>
    /// Kiểm tra booking có trong time frame không
    /// Booking times (UTC) được convert sang Vietnam timezone để so sánh
    /// </summary>
    private static bool IsBookingInTimeFrame(Booking booking, DateTimeOffset timeFrameStart, DateTimeOffset timeFrameEnd)
    {
        DateTimeOffset effectiveCheckIn;
        DateTimeOffset effectiveCheckOut;

        switch (booking.Status)
        {
            case BookingStatus.CheckedIn:
                // Đã CheckIn: dùng thời gian thực tế và convert sang Vietnam timezone
                var checkedInTime = booking.CheckedInAt ?? booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkedInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime);
                break;

            case BookingStatus.CheckedOut:
                // Đã CheckOut: không hiển thị
                return false;

            case BookingStatus.Confirmed:
            case BookingStatus.Pending:
            default:
                // Chưa CheckIn: dùng thời gian dự kiến và convert sang Vietnam timezone
                var checkInTime = booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime2 = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime2);
                break;
        }

        // Kiểm tra overlap với Vietnam timezone
        return effectiveCheckIn < timeFrameEnd && effectiveCheckOut > timeFrameStart;
    }

    /// <summary>
    /// Tạo calendar cell
    /// </summary>
    private static CalendarCell CreateCalendarCell(int timeFrameId, Booking? booking)
    {
        if (booking == null)
        {
            return new CalendarCell
            {
                TimeFrameId = timeFrameId,
                RoomStatus = RoomStatus.Available,
                ColSpan = 1,
                RowSpan = 1
            };
        }

        var roomStatus = booking.Status switch
        {
            BookingStatus.Pending => RoomStatus.Booked,
            BookingStatus.Confirmed => RoomStatus.Booked,
            BookingStatus.CheckedIn => RoomStatus.Using,
            _ => RoomStatus.Booked
        };

        return new CalendarCell
        {
            TimeFrameId = timeFrameId,
            BookingId = booking.Id,
            RoomStatus = roomStatus,
            BookingStatus = booking.Status,
            BookingStatusName = booking.Status.ToString(),
            PaymentStatus = booking.PaymentStatus,
            PaymentStatusName = booking.PaymentStatus.ToString(),
            CustomerId = booking.CustomerId,
            CustomerName = booking.CustomerName ?? booking.Customer?.FullName,
            CustomerPhone = booking.PhoneNumber ?? booking.Customer?.PhoneNumber,
            ColSpan = 1,
            RowSpan = 1
        };
    }
}
