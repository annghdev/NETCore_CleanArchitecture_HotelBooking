using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class PaymentTransactionRepository(BookingDbContext dbContext) : IPaymentTransactionRepository
{
    private readonly BookingDbContext _dbContext = dbContext;

    public async Task<IEnumerable<PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default!)
    {
        return await _dbContext.PaymentTransactions.AsNoTracking().ToListAsync();
    }

    public IQueryable<PaymentTransaction> GetQueryable()
    {
        return _dbContext.PaymentTransactions
            .AsNoTracking()
            .Include(p=>p.Booking);
    }
}
