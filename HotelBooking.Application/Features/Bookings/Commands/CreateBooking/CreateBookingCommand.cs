using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(CreateBookingRequest Booking) : IRequest<CreateBookingResult>;

public record CreateBookingRequest(
     int RoomId,
     Guid? CustomerId,
     string? SessionId,
     string? CustomerName,
     string? PhoneNumber,
     BookingType Type,
     BookingOrigin Origin,
     PaymentOrigin DepositOrigin,
     DateTime? CheckInDateTime,
     DateTime? CheckOutDateTime);
