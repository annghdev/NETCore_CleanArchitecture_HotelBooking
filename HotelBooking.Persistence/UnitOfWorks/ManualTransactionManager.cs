using HotelBooking.Domain;

namespace HotelBooking.Persistence.UnitOfWorks;

public class ManualTransactionManager : IUnitOfWork, IDisposable
{
    public Task BeginTransactionAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
