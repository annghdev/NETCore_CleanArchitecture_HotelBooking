using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking.ReceiveDepositBooking;

public record ReceiveDepositBookingCommand(
    Guid BookingId, 
    double Amount,
    PaymentOrigin Origin,
    string? TransactionNo = null) : IRequest<bool>;
