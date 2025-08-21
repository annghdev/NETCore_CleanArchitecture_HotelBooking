using MediatR;

namespace HotelBooking.Application.Features.Bookings.Queries.GetBookingById;

public record GetBookingByIdQuery(Guid Id) : IRequest<BookingVM>;
