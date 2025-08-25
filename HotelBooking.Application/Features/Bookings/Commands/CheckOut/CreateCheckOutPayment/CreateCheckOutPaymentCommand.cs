namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.CreateCheckOutPayment;

public record CreateCheckOutPaymentCommand(Guid BookingId, PaymentGateway PaymentOrigin) : IRequest<string>;
