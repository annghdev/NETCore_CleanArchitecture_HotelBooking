namespace HotelBooking.Application.ReadModels;

public class DateRangeCalendarMatrix
{
    public int RoomId { get; set; }
    public int RoomName { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public IEnumerable<TimeFrame> Columns { get; set; } = [];
    public IEnumerable<DateRangeCalendarRow> Rows { get; set; } = [];
    public int ColumnCount  => Columns.Count();
    public int RowCount  => Rows.Count();
}

public class DateRangeCalendarRow
{
    public DateTimeOffset Date { get; set; }
    public IEnumerable<CalendarCell> Cells { get; set; } = [];
}