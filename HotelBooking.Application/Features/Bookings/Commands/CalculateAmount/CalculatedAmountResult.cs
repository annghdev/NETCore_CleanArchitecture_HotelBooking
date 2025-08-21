namespace HotelBooking.Application.Features.Bookings.Commands.CalculateAmount;

public class CalculatedAmountResult(List<CalculatedRoomAmount> rooms)
{
    public List<CalculatedRoomAmount> Rooms { get; set; } = rooms;
    public double TotalAmount => Rooms.Sum(d => d.Amount);
}

public record CalculatedRoomAmountDetail(
    string Date,
    string DayOfWeek,
    string PolicyName,
    TimeOnly? From,
    TimeOnly? To,
    double Price,
    double Amount);

public class CalculatedRoomAmount
{
    public string RoomName { get; set; }
    public string RoomType { get; set; }
    public List<CalculatedRoomAmountDetail> Details { get; set; } = [];
    public double Amount => Details.Sum(d => d.Amount);

    public CalculatedRoomAmount(string roomName, string roomType, List<CalculatedRoomAmountDetail> details)
    {
        RoomName = roomName;
        RoomType = roomType;
        Details = details;
    }
}