namespace HotelBooking.Application.ReadModels;

public class CalendarRow
{
    public int RoomId { get; set; }
    public int RoomName { get; set; }
    public IEnumerable<CalendarCell> CalendarCells { get; set; } = [];
}
