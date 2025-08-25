using HotelBooking.Application.Features.RoomPickings.Models;

namespace HotelBooking.Application.Features.RoomPickings.Repository;

/// <summary>
/// Quản lý chọn/khóa phòng tạm thời để tránh conflict đặt phòng
/// </summary>
public interface IRoomPickingRepository
{
    Task<IEnumerable<RoomPicking>> GetAllAsync(Guid id);
    Task<RoomPicking?> GetByIdAsync(Guid id);
    Task<RoomPicking> CreateAsync(RoomPicking picking);
    Task UpdateAsync(RoomPicking picking);
    Task DeleteAsync(RoomPicking picking);
    Task<bool> EnsureNoConflictAsync(int roomId);
}
