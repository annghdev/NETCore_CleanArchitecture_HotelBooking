namespace HotelBooking.Application.Features.Calendar;

public abstract class CalendarBase
{
    public IEnumerable<TimeFrame> ColumnHeads { get; set; } = [];
    public int ColumnCount => ColumnHeads.Count();
    public virtual IEnumerable<CalendarRowBase> Rows { get; set; } = [];
    public int RowCount => Rows.Count();
}

public class TimeFrame
{
    public int Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double BasePrice { get; set; }
}

public abstract class CalendarRowBase
{
    public IEnumerable<CalendarCell> Cells { get; set; } = [];
}

public class CalendarCell
{
    public int TimeFrameId { get; set; }
    public Guid? BookingId { get; set; }
    public int ColSpan { get; set; }
    public int RowSpan { get; set; }
    public BookingStatus BookingStatus { get; set; }
    public string? BookingStatusName { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentStatusName { get; set; }
    public RoomStatus RoomStatus { get; set; } = RoomStatus.Available;
    public string RoomStatusName => RoomStatus.ToString();
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}

public enum RoomStatus
{
    Unavailable,
    Available,
    Booked,
    Locked,
    Using,
    Maintenancing
}