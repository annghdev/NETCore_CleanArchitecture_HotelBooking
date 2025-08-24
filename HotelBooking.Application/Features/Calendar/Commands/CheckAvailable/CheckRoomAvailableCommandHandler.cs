using HotelBooking.Application.Common;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Features.Calendar.Commands.CheckAvailable;

public class CheckRoomAvailableCommandHandler : IRequestHandler<CheckRoomAvailableCommand, RoomAvailableResult>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;

    public CheckRoomAvailableCommandHandler(
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<RoomAvailableResult> Handle(CheckRoomAvailableCommand request, CancellationToken cancellationToken)
    {
        // Normalize timezone inputs
        var normalizedRequest = request.NormalizeVietnamTimezone();

        // Kiểm tra phòng có tồn tại và active không
        var room = await _roomRepository.GetByIdAsync(normalizedRequest.RoomId);
        if (room == null || !room.IsActive)
        {
            return RoomAvailableResult.Overlap(Guid.Empty, "Room not found or inactive");
        }

        // Xác định thời gian kết thúc (Vietnam timezone)
        var fromTime = TimeZoneHelper.EnsureVietnamTimeZone(normalizedRequest.From);
        var toTime = normalizedRequest.To.HasValue 
            ? TimeZoneHelper.EnsureVietnamTimeZone(normalizedRequest.To.Value)
            : fromTime.AddHours(2); // Mặc định 2 giờ nếu không có To

        // Tìm các booking conflict
        var conflictingBookings = await _bookingRepository.GetQueryable()
            .Where(b => 
                // Booking ở trạng thái active
                (b.Status == BookingStatus.Confirmed ||
                 b.Status == BookingStatus.CheckedIn ||
                 b.Status == BookingStatus.Pending) &&
                // Có phòng này
                b.Rooms!.Any(br => br.RoomId == normalizedRequest.RoomId))
            .Include(b => b.Rooms)
            .ToListAsync(cancellationToken);

        // Kiểm tra từng booking xem có conflict không
        foreach (var booking in conflictingBookings)
        {
            if (IsBookingConflicting(booking, fromTime, toTime))
            {
                return RoomAvailableResult.Overlap(
                    booking.Id, 
                    booking.CustomerName ?? "Unknown customer");
            }
        }

        return RoomAvailableResult.Available();
    }

    /// <summary>
    /// Kiểm tra xem booking có conflict với khoảng thời gian yêu cầu không
    /// Booking times (UTC) được convert sang Vietnam timezone để so sánh
    /// </summary>
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

        // Ensure request times are in Vietnam timezone
        var requestFromVN = TimeZoneHelper.EnsureVietnamTimeZone(requestFrom);
        var requestToVN = TimeZoneHelper.EnsureVietnamTimeZone(requestTo);

        // Kiểm tra overlap: hai khoảng thời gian overlap nếu:
        // effectiveCheckIn < requestTo && effectiveCheckOut > requestFrom
        return effectiveCheckIn < requestToVN && effectiveCheckOut > requestFromVN;
    }
}
