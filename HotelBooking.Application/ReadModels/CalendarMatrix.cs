namespace HotelBooking.Application.ReadModels;

public class CalendarMatrix
{
    public DateTime DisplayDate { get; set; }
    public int Columns { get; set; } = 24;
    public int Rows { get; set; } = 4;
    public IEnumerable<TimeFrame>? TimeFrames { get; set; }
    public IEnumerable<CalendarRow>? CalendarRows { get; set; }
}
