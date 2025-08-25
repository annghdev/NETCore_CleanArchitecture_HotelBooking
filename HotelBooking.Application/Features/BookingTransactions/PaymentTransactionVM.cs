using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.BookingTransactions;

public class PaymentTransactionVM
{
    public DateTimeOffset? OccuredDate { get; set; }
    public double Amount { get; set; }
    public PaymentAction Type { get; set; }
    public string? TypeName { get; set; }
    public PaymentGateway Origin { get; set; }
    public string? OriginName { get; set; }
    public string? TransactionNo { get; set; }
    public PaymentProcessStatus ProcessStatus { get; set; }
    public string? ProcessStatusName { get; set; }
}
