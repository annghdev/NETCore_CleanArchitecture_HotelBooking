using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class PricingPolicyRepository(BookingDbContext dbContext) 
    : RepositoryBase<PricingPolicy, int>(dbContext), IPricingPolicyRepository
{
    public override async Task<PricingPolicy?> GetByIdAsync(int id)
    {
        return await dbSet.SingleOrDefaultAsync(p => p.Id == id);
    }
}
