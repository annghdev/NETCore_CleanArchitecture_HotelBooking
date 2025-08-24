namespace HotelBooking.Application.Features.Bookings.Commands.CheckIn;

public record CheckInCommand(Guid BookingId, string IdentityNo) : IRequest<bool>;