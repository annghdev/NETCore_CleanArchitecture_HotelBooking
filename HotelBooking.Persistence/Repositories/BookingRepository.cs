using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class BookingRepository(BookingDbContext dbContext) 
    : RepositoryBase<Booking, Guid>(dbContext), IBookingRepository
{
    public async Task<Booking> DraftAsync(Booking booking)
    {
        booking.DraftedDate = DateTimeOffset.UtcNow;
        await dbContext.Bookings.AddAsync(booking);
        await dbContext.SaveChangesAsync();
        return booking;
    }

    public override async Task<Booking?> AddAsync(Booking entity)
    {
        entity.CreatedDate = DateTimeOffset.UtcNow;
        dbContext.Update(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async override Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet.Include(b=>b.Rooms).ToListAsync(cancellationToken);
    }

    public override async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await dbSet
            .Include(b => b.Rooms)!
                .ThenInclude(r=> r.Room)
            .Include(b => b.Transactions)
            .SingleOrDefaultAsync(p => p.Id == id);
    }
}
