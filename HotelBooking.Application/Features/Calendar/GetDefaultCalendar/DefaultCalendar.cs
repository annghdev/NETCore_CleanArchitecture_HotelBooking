namespace HotelBooking.Application.Features.Calendar.GetDefaultCalendar;

public class DefaultCalendar : CalendarBase
{
    public DateTime DisplayDate { get; set; }
    public override IEnumerable<CalendarRowBase> Rows { get; set; } = new List<DefaultCalendarRow>();
}

public class DefaultCalendarRow : CalendarRowBase
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = default!;
}

