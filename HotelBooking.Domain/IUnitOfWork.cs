namespace HotelBooking.Domain;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task<int> SaveChangesAsync();
}
