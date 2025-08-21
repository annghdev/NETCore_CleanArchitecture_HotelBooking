using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelBooking.Persistence.Repositories;

public class UserRepository(BookingDbContext dbContext) 
    : RepositoryBase<User, Guid>(dbContext), IUserRepository
{
    public override Task<User?> GetByIdAsync(Guid id)
    {
        return dbSet
            .Include(u => u.Roles)!
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
            .Include(u => u.Permissions)!
                .ThenInclude(p => p.Permission)
            .SingleOrDefaultAsync(u => u.Id == id);
    }

    public Task<User?> GetSingleAsync(Expression<Func<User, bool>> predicate)
    {
        return dbSet.SingleOrDefaultAsync(predicate);
    }
}
