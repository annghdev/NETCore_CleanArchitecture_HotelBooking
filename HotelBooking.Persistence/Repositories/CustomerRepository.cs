using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class CustomerRepository(BookingDbContext dbContext) : RepositoryBase<Customer, Guid>(dbContext), ICustomerRepository
{
    public override async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet.AsNoTracking()
            .Include(p => p.User)
            .ToListAsync(cancellationToken);
    }
    public override async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await dbSet.AsNoTracking()
            .Include(p => p.User)
            .SingleOrDefaultAsync(p => p.Id == id);
    }
}
