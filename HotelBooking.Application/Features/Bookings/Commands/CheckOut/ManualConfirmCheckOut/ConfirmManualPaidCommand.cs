namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.ManualConfirmCheckOut;

public record ConfirmManualPaidCommand(Guid BookingId, double Amount, PaymentGateway Origin) : IRequest;
