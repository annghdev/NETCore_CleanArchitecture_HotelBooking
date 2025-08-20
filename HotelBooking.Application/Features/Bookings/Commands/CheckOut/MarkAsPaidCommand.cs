using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CheckOut;

public record MarkAsPaidCommand(Guid BookingId, double Amount, PaymentOrigin Origin) : IRequest;
