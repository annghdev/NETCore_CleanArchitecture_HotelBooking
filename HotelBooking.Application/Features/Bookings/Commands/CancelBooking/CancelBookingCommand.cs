namespace HotelBooking.Application.Features.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(
    Guid BookingId,
    string Reason,
    bool ForceCancel = false) : IRequest<BookingVM>;
