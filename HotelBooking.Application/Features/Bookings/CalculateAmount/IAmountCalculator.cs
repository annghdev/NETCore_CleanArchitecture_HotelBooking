namespace HotelBooking.Application.Features.Bookings.CalculateAmount;


public interface IAmountCalculator
{
    Task<AmountResult> CalculateAmount(Booking booking);
}
public class AmountResult(List<RoomAmount> rooms)
{
    public List<RoomAmount> Rooms { get; set; } = rooms;
    public double TotalAmount => Rooms.Sum(d => d.Amount);
}

public record RoomAmountDetail(
    string Date,
    string DayOfWeek,
    string PolicyName,
    TimeOnly? From,
    TimeOnly? To,
    double Price,
    double Amount);

public class RoomAmount
{
    public string RoomName { get; set; }
    public string RoomType { get; set; }
    public List<RoomAmountDetail> Details { get; set; } = [];
    public double Amount => Details.Sum(d => d.Amount);

    public RoomAmount(string roomName, string roomType, List<RoomAmountDetail> details)
    {
        RoomName = roomName;
        RoomType = roomType;
        Details = details;
    }
}
