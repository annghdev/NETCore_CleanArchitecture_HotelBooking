namespace HotelBooking.Domain.Abstractions;

public interface IUnitOfWork
{
    Task BeginAsync();
    Task<int> CommitAsync();
}
