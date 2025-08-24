namespace HotelBooking.Application.ReadModels;

public class CalendarRow
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = default!;
    public IEnumerable<CalendarCell> Cells { get; set; } = [];
}
