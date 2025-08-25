namespace HotelBooking.Application.Features.BookingTransactions.Commands.ReceivePaymentResponse;

public record ReceivePaymentResponseCommand(
    Guid BookingId, 
    double Amount,
    PaymentAction Type,
    PaymentGateway Origin,
    string? TransactionNo = null) : IRequest<bool>;
