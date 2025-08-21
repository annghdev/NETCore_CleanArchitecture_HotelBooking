using HotelBooking.Domain.Entities;

namespace HotelBooking.Domain.Repositories;

public interface IPaymentTransactionRepository
{
    Task<IEnumerable<PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default!);
    IQueryable<PaymentTransaction> GetQueryable();
}
