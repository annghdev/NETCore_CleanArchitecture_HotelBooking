using HotelBooking.Application.Common;
using HotelBooking.Domain.Common;
using MediatR;

namespace HotelBooking.Application.Features.Payments.Queries.PaginPaymentTransactions;

public class PaginPaymentTransactionsQuery : PaginRequest, IRequest<PagedResult<PaymentTransactionVM>>
{
    public DateTimeOffset? OccuredDate  { get; set; }
    public DateTimeOffset? OccuredDateTo  { get; set; }
    public SearchDateType SearchDateType  { get; set; }
}
