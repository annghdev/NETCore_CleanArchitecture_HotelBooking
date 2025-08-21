using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record CreateCheckOutPaymentCommand(Guid BookingId) : IRequest<string>;
