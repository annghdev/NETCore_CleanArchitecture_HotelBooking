using HotelBooking.Domain.Entities.Bases;
using HotelBooking.Domain.Repositories;
using HotelBooking.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Persistence.Repositories;

public abstract class RepositoryBase<TEntity, TKey>(BookingDbContext dbContext) 
    : IRepositoryBase<TEntity, TKey>
    where TEntity : class, IAggregateRoot
{
    private readonly BookingDbContext _dbContext = dbContext;
    protected readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<TEntity?> AddAsync(TEntity entity)
    {
        await _dbContext.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Deleted;
        return Task.CompletedTask;
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return dbSet.AsQueryable();
    }
}
