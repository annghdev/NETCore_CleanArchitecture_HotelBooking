using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Payments.Commands.ReceiveDepositBooking;

public record ReceivePaymentResultCommand(
    Guid BookingId, 
    double Amount,
    PaymentType Type,
    PaymentOrigin Origin,
    string? TransactionNo = null) : IRequest<bool>;
