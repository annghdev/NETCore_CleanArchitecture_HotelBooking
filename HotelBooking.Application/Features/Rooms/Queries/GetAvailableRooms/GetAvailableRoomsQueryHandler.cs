using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Rooms.Queries.GetAvailableRooms;

public class GetAvailableRoomsQueryHandler : IRequestHandler<GetAvailableRoomsQuery, IEnumerable<RoomVM>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetAvailableRoomsQueryHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IMapper mapper)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoomVM>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
    {
        // Normalize timezone inputs
        var normalizedRequest = request.NormalizeVietnamTimezone();
        
        // Ensure request times có proper Vietnam timezone
        var requestFromVN = TimeZoneHelper.EnsureVietnamTimeZone(normalizedRequest.From);
        var requestToVN = TimeZoneHelper.EnsureVietnamTimeZone(normalizedRequest.To);

        // Lấy tất cả phòng đang active
        var allActiveRooms = await _roomRepository.GetQueryable()
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);

        // Lấy tất cả booking active với thông tin phòng
        // Database query với UTC times, sẽ convert sau khi lấy data
        var activeBookings = await _bookingRepository.GetQueryable()
            .Where(b => 
                // Booking phải ở trạng thái active (không phải cancelled, noshow, hoặc draft)
                b.Status == BookingStatus.Confirmed ||
                b.Status == BookingStatus.CheckedIn ||
                b.Status == BookingStatus.Pending)
            .Include(b => b.Rooms)
            .ToListAsync(cancellationToken);

        // Tìm các phòng bị conflict với thời gian yêu cầu (Vietnam timezone)
        var occupiedRoomIds = new HashSet<int>();
        
        foreach (var booking in activeBookings)
        {
            if (IsBookingConflicting(booking, requestFromVN, requestToVN))
            {
                if (booking.Rooms != null)
                {
                    foreach (var bookingRoom in booking.Rooms)
                    {
                        occupiedRoomIds.Add(bookingRoom.RoomId);
                    }
                }
            }
        }

        // Lọc ra các phòng không bị chiếm
        var availableRooms = allActiveRooms
            .Where(r => !occupiedRoomIds.Contains(r.Id))
            .ToList();

        return _mapper.Map<IEnumerable<RoomVM>>(availableRooms);
    }

    /// <summary>
    /// Kiểm tra xem booking có conflict với khoảng thời gian yêu cầu không
    /// Booking times (UTC) được convert sang Vietnam timezone để so sánh
    /// </summary>
    /// <param name="booking">Booking cần kiểm tra (UTC times from database)</param>
    /// <param name="requestFrom">Thời gian bắt đầu yêu cầu (Vietnam timezone)</param>
    /// <param name="requestTo">Thời gian kết thúc yêu cầu (Vietnam timezone)</param>
    /// <returns>True nếu có conflict</returns>
    private static bool IsBookingConflicting(Booking booking, DateTimeOffset requestFrom, DateTimeOffset requestTo)
    {
        DateTimeOffset effectiveCheckIn;
        DateTimeOffset effectiveCheckOut;

        switch (booking.Status)
        {
            case BookingStatus.CheckedIn:
                // Đã CheckIn: dùng thời gian CheckIn thực tế, CheckOut dự kiến và convert sang Vietnam timezone
                var checkedInTime = booking.CheckedInAt ?? booking.CheckInDateTime ?? DateTimeOffset.MinValue;
                var checkOutTime = booking.CheckOutDateTime ?? DateTimeOffset.MaxValue;
                effectiveCheckIn = TimeZoneHelper.ToVietnamTime(checkedInTime);
                effectiveCheckOut = TimeZoneHelper.ToVietnamTime(checkOutTime);
                break;

            case BookingStatus.CheckedOut:
                // Đã CheckOut: phòng đã trống, không conflict
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

        // Kiểm tra overlap trong Vietnam timezone: hai khoảng thời gian overlap nếu:
        // effectiveCheckIn < requestTo && effectiveCheckOut > requestFrom
        return effectiveCheckIn < requestTo && effectiveCheckOut > requestFrom;
    }
}
