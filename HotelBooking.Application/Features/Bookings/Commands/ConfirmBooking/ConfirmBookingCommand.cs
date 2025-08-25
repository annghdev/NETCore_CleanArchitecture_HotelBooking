namespace HotelBooking.Application.Features.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(
    Guid BookingId,
    string? CustomerName = null,
    string? PhoneNumber = null,
    string? Notes = null,
    bool SkipPaymentValidation = false) : IRequest<BookingVM>;
