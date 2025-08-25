using HotelBooking.Application.Features.RoomPickings.Models;
using HotelBooking.Application.Features.RoomPickings.Repository;

namespace HotelBooking.Application.Features.RoomPickings.Commands.PickRoom;

public class PickRoomCommandHandler : IRequestHandler<PickRoomCommand, bool>
{
    private readonly IRoomPickingRepository _roomPickingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBusinessSettingRepository _businessSettingRepository;

    public PickRoomCommandHandler(
        IRoomPickingRepository roomPickingRepository,
        IUnitOfWork unitOfWork,
        IBusinessSettingRepository businessSettingRepository)
    {
        _roomPickingRepository = roomPickingRepository;
        _unitOfWork = unitOfWork;
        _businessSettingRepository = businessSettingRepository;
    }

    /// <summary>
    /// Chọn/khóa phòng tạm thời để tránh conflict khi có nhiều người đặt cùng lúc
    /// </summary>
    public async Task<bool> Handle(PickRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Kiểm tra phòng có tồn tại và khả dụng không
            await ValidateRoomAvailability(request.RoomId, request.CheckInDateTime, request.CheckOutDateTime);

            // Kiểm tra phòng có bị người khác pick không
            var hasConflict = await _roomPickingRepository.EnsureNoConflictAsync(request.RoomId);
            if (hasConflict)
            {
                throw new DomainException("Phòng đang được chọn bởi người dùng khác");
            }

            // Lấy business setting để tính auto release time
            var businessSetting = await _businessSettingRepository.GetSetting() 
                ?? throw new DomainException("Không tìm thấy cài đặt hệ thống");

            var autoReleaseTime = DateTimeOffset.UtcNow.AddMinutes(businessSetting.DraftExpiryMinutes);

            RoomPicking roomPicking;

            if (request.PickingId.HasValue)
            {
                // Cập nhật picking existing
                roomPicking = await _roomPickingRepository.GetByIdAsync(request.PickingId.Value);
                if (roomPicking == null)
                {
                    throw new NotFoundException($"Không tìm thấy room picking với ID: {request.PickingId}");
                }

                // Cập nhật thông tin
                var roomIds = roomPicking.RoomIds.ToList();
                if (!roomIds.Contains(request.RoomId))
                {
                    roomIds.Add(request.RoomId);
                }

                roomPicking.RoomIds = roomIds;
                roomPicking.AutoRealeaseDate = autoReleaseTime;
                roomPicking.Type = request.BookingType;
                roomPicking.Origin = request.Origin;

                await _roomPickingRepository.UpdateAsync(roomPicking);
            }
            else
            {
                // Tạo picking mới
                roomPicking = new RoomPicking
                {
                    Id = Guid.NewGuid(),
                    SessionId = request.SessionId,
                    CustomerId = request.CustomerId,
                    Type = request.BookingType,
                    Origin = request.Origin,
                    AutoRealeaseDate = autoReleaseTime,
                    RoomIds = [request.RoomId]
                };

                await _roomPickingRepository.CreateAsync(roomPicking);
            }

            return true;
        }
        catch (Exception ex) when (ex is not DomainException and not NotFoundException)
        {
            // Log error
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra tính khả dụng của phòng
    /// </summary>
    private async Task ValidateRoomAvailability(int roomId, DateTimeOffset checkIn, DateTimeOffset checkOut)
    {
        // Kiểm tra phòng có tồn tại không
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId);
        if (room == null)
            throw new NotFoundException($"Không tìm thấy phòng với ID: {roomId}");

        if (!room.IsActive)
            throw new DomainException($"Phòng {room.Name} hiện không khả dụng");

        // Kiểm tra phòng có bị trùng lịch với booking đã confirmed không
        var conflictBookings = _unitOfWork.BookingRepository.GetQueryable()
            .Where(b => b.Rooms!.Any(br => br.RoomId == roomId) &&
                       (b.Status == BookingStatus.Confirmed || 
                        b.Status == BookingStatus.CheckedIn) &&
                       b.CheckInDateTime < checkOut &&
                       b.CheckOutDateTime > checkIn)
            .Any();

        if (conflictBookings)
            throw new DomainException($"Phòng {room.Name} đã được đặt trong khoảng thời gian này");
    }
}
