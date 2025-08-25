using HotelBooking.Application.Features.RoomPickings.Models;
using HotelBooking.Application.Features.RoomPickings.Repository;

namespace HotelBooking.Infrastructure.RoomPickings;

public class RoomPickingRepository : IRoomPickingRepository
{
    public Task<RoomPicking> CreateAsync(RoomPicking picking)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(RoomPicking picking)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EnsureNoConflictAsync(int roomId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoomPicking>> GetAllAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<RoomPicking?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(RoomPicking picking)
    {
        throw new NotImplementedException();
    }
}
