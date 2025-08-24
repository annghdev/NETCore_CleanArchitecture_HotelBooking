using MediatR;

namespace HotelBooking.Application.Features.Calendar.GetDefaultCalendar;

public record GetDefaultCalendarQuery(DateTimeOffset Date) : IRequest<DefaultCalendar>;
