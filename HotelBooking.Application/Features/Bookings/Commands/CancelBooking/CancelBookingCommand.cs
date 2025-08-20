using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(Guid BookingId) : IRequest<bool>;
