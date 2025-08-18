namespace HotelBooking.Application.ReadModels;

public class TimeFrame
{
    public int Id { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double BasePrice { get; set; }
}
