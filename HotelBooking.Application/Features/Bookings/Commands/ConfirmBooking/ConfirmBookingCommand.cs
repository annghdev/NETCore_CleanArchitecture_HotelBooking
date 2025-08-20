using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(Guid BookingId) : IRequest;
