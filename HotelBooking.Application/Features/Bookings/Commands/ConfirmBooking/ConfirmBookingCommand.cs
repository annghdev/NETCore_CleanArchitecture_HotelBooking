namespace HotelBooking.Application.Features.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(Guid BookingId) : IRequest<ConfirmBookingResult>;

public record ConfirmBookingResult(bool IsSuccess, string Message);
