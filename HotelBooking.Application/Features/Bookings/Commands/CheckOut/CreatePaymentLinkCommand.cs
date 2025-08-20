using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record CreatePaymentLinkCommand(Guid BookingId) : IRequest<string>;
