using HotelBooking.Domain.Entities;
using System.Linq.Expressions;

namespace HotelBooking.Domain.Repositories;

public interface IUserRepository : IRepositoryBase<User, Guid>
{
    Task<User?> GetSingleAsync(Expression<Func<User, bool>> predicate);
}
