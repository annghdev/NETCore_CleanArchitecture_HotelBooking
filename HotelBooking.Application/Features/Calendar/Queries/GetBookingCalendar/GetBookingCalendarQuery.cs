using HotelBooking.Application.ReadModels;
using MediatR;

namespace HotelBooking.Application.Features.Calendar.Queries.GetBookingCalendar;

public record GetBookingCalendarQuery(DateTimeOffset Date) : IRequest<CalendarMatrix>;
