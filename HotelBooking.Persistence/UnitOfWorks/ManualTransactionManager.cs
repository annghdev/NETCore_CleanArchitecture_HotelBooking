using HotelBooking.Domain;

namespace HotelBooking.Persistence.UnitOfWorks;

public class ManualTransactionManager : IUnitOfWork, IDisposable
{
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
