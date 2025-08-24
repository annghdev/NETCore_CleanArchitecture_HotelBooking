namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record CreateCheckOutPaymentCommand(Guid BookingId, PaymentOrigin PaymentOrigin) : IRequest<string>;
