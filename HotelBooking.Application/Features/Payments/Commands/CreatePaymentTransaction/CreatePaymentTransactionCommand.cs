using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Payments.Commands.CreatePaymentTransaction;

public record CreatePaymentTransactionCommand(
    Guid BookingId,
    PaymentType Type,
    PaymentOrigin Origin,
    double Amount,
    string Information,
    string GateWay) : IRequest<(bool, string)>;
