using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record CheckOutCommand(Guid BookingId) : IRequest<AmountResult>;
