using HotelBooking.Application.Features.RoomPickings.Repository;

namespace HotelBooking.Application.Features.RoomPickings.Commands.ReleasePickedRoom;

public class ReleasePickedRoomCommandHandler : IRequestHandler<ReleasePickedRoomCommand, bool>
{
    private readonly IRoomPickingRepository _roomPickingRepository;

    public ReleasePickedRoomCommandHandler(IRoomPickingRepository roomPickingRepository)
    {
        _roomPickingRepository = roomPickingRepository;
    }

    /// <summary>
    /// Giải phóng phòng đã pick (bỏ chọn phòng)
    /// </summary>
    public async Task<bool> Handle(ReleasePickedRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Lấy room picking
            var roomPicking = await _roomPickingRepository.GetByIdAsync(request.PickingId);
            if (roomPicking == null)
            {
                // Không tìm thấy picking, có thể đã được release hoặc expired
                return true;
            }

            // Xóa room khỏi picking
            var roomIds = roomPicking.RoomIds.ToList();
            roomIds.Remove(request.RoomId);

            if (!roomIds.Any())
            {
                // Nếu không còn room nào thì xóa luôn picking
                await _roomPickingRepository.DeleteAsync(roomPicking);
            }
            else
            {
                // Cập nhật picking với danh sách room mới
                roomPicking.RoomIds = roomIds;
                await _roomPickingRepository.UpdateAsync(roomPicking);
            }

            return true;
        }
        catch (Exception)
        {
            // Log error
            return false;
        }
    }
}
