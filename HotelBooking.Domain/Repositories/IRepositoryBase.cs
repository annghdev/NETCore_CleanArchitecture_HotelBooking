using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Repositories;

public interface IRepositoryBase<TEntity, Tkey> where TEntity : IAggregateRoot
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default!);
    Task<TEntity?> GetByIdAsync(Tkey id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);

    IQueryable<TEntity> GetQueryable();
}
