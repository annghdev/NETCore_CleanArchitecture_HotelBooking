using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckIn;

public record CheckInCommand(Guid BookingId) : IRequest<bool>;