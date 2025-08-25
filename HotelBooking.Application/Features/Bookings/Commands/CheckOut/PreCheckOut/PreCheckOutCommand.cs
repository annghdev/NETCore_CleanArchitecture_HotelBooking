using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut.PreCheckOut;

public record PreCheckOutCommand(Guid BookingId) : IRequest<AmountResult>;
