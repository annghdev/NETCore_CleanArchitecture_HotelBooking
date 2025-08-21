namespace HotelBooking.Application.ReadModels;

public class CalendarMatrix
{
    public DateTime DisplayDate { get; set; }
    public IEnumerable<TimeFrame> Columns { get; set; } = [];
    public IEnumerable<CalendarRow> Rows { get; set; } = [];
    public int ColumnCount => Columns.Count();
    public int RowCount => Rows.Count();
}
