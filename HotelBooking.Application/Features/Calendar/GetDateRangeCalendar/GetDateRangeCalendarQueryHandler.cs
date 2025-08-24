using HotelBooking.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Calendar.GetDateRangeCalendar;

public class GetDateRangeCalendarQueryHandler : IRequestHandler<GetDateRangeCalendarQuery, DateRangeCalendar>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public GetDateRangeCalendarQueryHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<DateRangeCalendar> Handle(GetDateRangeCalendarQuery request, CancellationToken cancellationToken)
    {
        // Kiểm tra phòng có tồn tại không
        var room = await _roomRepository.GetByIdAsync(request.RoomId);
        if (room == null)
        {
            throw new NotFoundException("Room", request.RoomId.ToString());
        }

        // Tạo time frames cho ngày (theo giờ)
        var timeFrames = GenerateTimeFrames();

        // Tính toán date range theo Vietnam timezone
        var rangeStart = TimeZoneHelper.GetDateOnlyStartUtc(request.From);
        var rangeEnd = TimeZoneHelper.GetDateOnlyEndUtc(request.To);
        
        // Tạo Vietnam DateTimeOffset cho display
        var vietnamFrom = TimeZoneHelper.ToVietnamDateTimeOffset(request.From);
        var vietnamTo = TimeZoneHelper.ToVietnamDateTimeOffset(request.To, TimeOnly.MaxValue);

        // Lấy tất cả booking cho phòng này trong khoảng thời gian (query với UTC)
        var bookings = await _bookingRepository.GetQueryable()
            .Where(b => 
                // Booking active
                (b.Status == BookingStatus.Confirmed ||
                 b.Status == BookingStatus.CheckedIn ||
                 b.Status == BookingStatus.Pending) &&
                // Có phòng này
                b.Rooms!.Any(br => br.RoomId == request.RoomId) &&
                // Overlap với khoảng thời gian (so sánh với UTC times trong database)
                b.CheckInDateTime < rangeEnd &&
                b.CheckOutDateTime > rangeStart)
            .Include(b => b.Rooms)
            .Include(b => b.Customer)
            .ToListAsync(cancellationToken);

        // Tạo rows cho từng ngày
        var rows = new List<DateRangeCalendarRow>();

        for (var currentDate = request.From; currentDate <= request.To; currentDate = currentDate.AddDays(1))
        {
            // Tạo Vietnam day range
            var vietnamDayStart = TimeZoneHelper.ToVietnamDateTimeOffset(currentDate);
            var dayStartUtc = TimeZoneHelper.GetDateOnlyStartUtc(currentDate);
            var dayEndUtc = TimeZoneHelper.GetDateOnlyEndUtc(currentDate);

            // Lọc bookings cho ngày này (so sánh với UTC times)
            var dayBookings = bookings.Where(b => 
                b.CheckInDateTime < dayEndUtc && 
                b.CheckOutDateTime > dayStartUtc).ToList();

            var cells = CreateMergedCellsForDay(timeFrames, dayBookings, vietnamDayStart);

            rows.Add(new DateRangeCalendarRow
            {
                Date = vietnamDayStart,
                Cells = cells
            });
        }

        return new DateRangeCalendar
        {
            RoomId = room.Id,
            RoomName = room.Name,
            From = vietnamFrom,
            To = vietnamTo,
            ColumnHeads = timeFrames,
            Rows = rows
        };
    }

    /// <summary>
    /// Tạo cells cho một ngày với logic merge cells cùng bookingId
    /// </summary>
    private static List<CalendarCell> CreateMergedCellsForDay(
        List<TimeFrame> timeFrames, 
        List<Booking> dayBookings, 
        DateTimeOffset vietnamDayStart)
    {
        var cells = new List<CalendarCell>();
        var processedTimeFrames = new HashSet<int>();

        foreach (var timeFrame in timeFrames)
        {
            if (processedTimeFrames.Contains(timeFrame.Id))
                continue;

            // Tạo time frame theo Vietnam timezone
            var timeFrameStart = vietnamDayStart.AddHours(timeFrame.StartTime.Hour).AddMinutes(timeFrame.StartTime.Minute);
            var timeFrameEnd = vietnamDayStart.AddHours(timeFrame.EndTime.Hour).AddMinutes(timeFrame.EndTime.Minute);

            // Tìm booking trong time frame này
            var booking = dayBookings.FirstOrDefault(b => 
                IsBookingInTimeFrame(b, timeFrameStart, timeFrameEnd));

            if (booking == null)
            {
                // Không có booking, tạo cell thường
                cells.Add(CreateCalendarCell(timeFrame.Id, null));
                processedTimeFrames.Add(timeFrame.Id);
            }
            else
            {
                // Có booking, tính toán colSpan để merge cells
                var colSpan = CalculateColSpanForBooking(booking, timeFrame, timeFrames, vietnamDayStart);
                
                // Tạo cell đầu tiên với colSpan
                var mergedCell = CreateCalendarCell(timeFrame.Id, booking);
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
    /// Tính toán ColSpan cho một booking trong một ngày
    /// </summary>
    private static int CalculateColSpanForBooking(
        Booking booking, 
        TimeFrame startTimeFrame, 
        List<TimeFrame> allTimeFrames, 
        DateTimeOffset vietnamDayStart)
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

        // Giới hạn thời gian trong ngày hiện tại (Vietnam timezone)
        var vietnamDayEnd = vietnamDayStart.AddDays(1);
        effectiveCheckIn = effectiveCheckIn < vietnamDayStart ? vietnamDayStart : effectiveCheckIn;
        effectiveCheckOut = effectiveCheckOut > vietnamDayEnd ? vietnamDayEnd : effectiveCheckOut;

        // Đếm số time frames liên tiếp mà booking này chiếm
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
                var checkedInTime = booking.CheckedInAt ?? booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkedInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime);
                break;
            case BookingStatus.CheckedOut:
                return false;
            default:
                var checkInTime = booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime2 = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime2);
                break;
        }

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
}
