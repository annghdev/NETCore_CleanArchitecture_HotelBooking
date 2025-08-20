using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public class RoleRepository(BookingDbContext dbContext) 
    : RepositoryBase<Role, Guid>(dbContext), IRoleRepository
{
    public override async Task<Role?> GetByIdAsync(Guid id)
    {
        return await dbSet.AsNoTracking()
            .Include(r=>r.Permissions)
                .ThenInclude(p=>p.Permission)
            .SingleOrDefaultAsync(r => r.Id == id);
    }
    public override async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet.AsNoTracking()
            .Include(r => r.Permissions)
                .ThenInclude(p => p.Permission)
            .ToListAsync(cancellationToken);
    }
}
