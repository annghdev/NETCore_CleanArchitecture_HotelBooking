namespace HotelBooking.Domain.Repositories;

public interface IUnitOfWork
{
    Task BeginAsync();
    Task<int> CommitAsync();
}
