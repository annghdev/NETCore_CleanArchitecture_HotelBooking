using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class UserRepository(BookingDbContext dbContext) : RepositoryBase<User, Guid>(dbContext), IUserRepository
{
    public override Task<User?> GetByIdAsync(Guid id)
    {
        return dbSet.AsNoTracking()
            .Include(u => u.Roles)
                .ThenInclude(r => r.Role)
                    .ThenInclude(r => r.Permissions)
            .Include(u => u.Permissions)
                .ThenInclude(p => p.Permission)
            .SingleOrDefaultAsync(u => u.Id == id);
    }
}
