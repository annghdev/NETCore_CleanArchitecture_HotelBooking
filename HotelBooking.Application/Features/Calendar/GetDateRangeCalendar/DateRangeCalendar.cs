using HotelBooking.Application.Features.Calendar.GetDefaultCalendar;

namespace HotelBooking.Application.Features.Calendar.GetDateRangeCalendar;

public class DateRangeCalendar : CalendarBase
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = default!;
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public override IEnumerable<CalendarRowBase> Rows { get; set; } = new List<DefaultCalendarRow>();
}

public class DateRangeCalendarRow : CalendarRowBase
{
    public DateTimeOffset Date { get; set; }
}