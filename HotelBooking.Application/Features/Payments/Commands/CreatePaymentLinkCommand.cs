using MediatR;

namespace HotelBooking.Application.Features.Payments.Commands;

public record CreatePaymentLinkCommand(
    Guid BookingId,
    double Amount,
    string Information,
    string GateWay) : IRequest<string>;
