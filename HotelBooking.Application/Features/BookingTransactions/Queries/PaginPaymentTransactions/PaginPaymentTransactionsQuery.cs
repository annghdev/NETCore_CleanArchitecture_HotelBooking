using HotelBooking.Application.Common.Utils;
using HotelBooking.Application.Features.BookingTransactions;
using MediatR;

namespace HotelBooking.Application.Features.BookingTransactions.Queries.PaginPaymentTransactions;

public class PaginPaymentTransactionsQuery : PaginRequest, IRequest<PagedResult<PaymentTransactionVM>>
{
    public DateTimeOffset? OccuredDate  { get; set; }
    public DateTimeOffset? OccuredDateTo  { get; set; }
    public SearchDateType SearchDateType  { get; set; }
}
