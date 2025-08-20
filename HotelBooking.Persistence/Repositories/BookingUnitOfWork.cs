using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;

namespace HotelBooking.Persistence.Repositories;

public class BookingUnitOfWork(BookingDbContext dbContext) : IUnitOfWork
{
    private readonly BookingDbContext _dbContext = dbContext;

    public Task BeginAsync()
    {
        return Task.CompletedTask;
    }

    public Task<int> CommitAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
