using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class BookingRepository(BookingDbContext dbContext) : RepositoryBase<Booking, Guid>(dbContext), IBookingRepository
{
    public override async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await dbSet.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
    }
}
