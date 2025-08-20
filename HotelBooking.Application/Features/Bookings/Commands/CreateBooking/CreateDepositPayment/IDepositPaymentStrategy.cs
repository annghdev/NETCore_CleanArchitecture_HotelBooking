using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking.CreateDepositPayment;

public interface IDepositPaymentStrategy
{
    Task<CreateBookingResult> HandleAsync(Booking booking);
}
