namespace HotelBooking.Application.Features.Calendar.GetDateRangeCalendar;

public record GetDateRangeCalendarQuery(
    int RoomId,
    DateOnly From,
    DateOnly To) : IRequest<DateRangeCalendar>;
