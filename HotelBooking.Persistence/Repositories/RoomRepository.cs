using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class RoomRepository(BookingDbContext dbContext) 
    : RepositoryBase<Room, int>(dbContext), IRoomRepository
{
    public override async Task<Room?> GetByIdAsync(int id)
    {
        return await dbSet.AsNoTracking().SingleOrDefaultAsync(r => r.Id == id);
    }
}
