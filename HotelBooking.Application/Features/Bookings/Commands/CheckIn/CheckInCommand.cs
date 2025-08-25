namespace HotelBooking.Application.Features.Bookings.Commands.CheckIn;

public record CheckInCommand(
    Guid BookingId,
    string CustomerName,
    string PhoneNumber,
    DateTimeOffset? ActualCheckInTime = null,
    string? Notes = null) : IRequest<BookingVM>;