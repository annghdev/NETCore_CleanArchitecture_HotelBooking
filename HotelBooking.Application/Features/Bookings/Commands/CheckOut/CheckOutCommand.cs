using HotelBooking.Application.Features.Bookings.Commands.CalculateAmount;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record CheckOutCommand(Guid BookingId) : IRequest<CalculatedAmountResult>;
