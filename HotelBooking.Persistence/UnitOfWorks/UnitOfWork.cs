using HotelBooking.Domain;
using HotelBooking.Domain.Entities.Bases;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.UnitOfWorks;

public class UnitOfWork(BookingDbContext dbContext, IUserContext userContext) : IUnitOfWork
{
    private readonly BookingDbContext _dbContext = dbContext;
    private readonly IUserContext _userContext = userContext;
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAudit();
        int rows = await _dbContext.SaveChangesAsync(cancellationToken);
        DispatchEvents();
        return rows;
    }
    private void DispatchEvents()
    {
        //
    }

    private void ApplyAudit()
    {
        ApplyUserTracking();
        ApplyDateTracking();
    }
    private void ApplyUserTracking()
    {
        var entries = _dbContext.ChangeTracker.Entries<IUserTracking>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _userContext.UserId;
                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    entry.Entity.UpdatedBy = _userContext.UserId;
                    break;
            }
        }
    }
    private void ApplyDateTracking()
    {
        var entries = _dbContext.ChangeTracker.Entries<IDateTracking>();

        foreach (var entry in entries)
        {
            var now = DateTimeOffset.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedDate = now;
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDeletable softDeletableEntity)
                    {
                        softDeletableEntity.DeletedDate = now;
                        entry.State = EntityState.Modified;
                    }
                    break;
            }
        }
    }
}
