namespace HotelBooking.Application.Features.Bookings.Commands.ChangeRoom;

public record ChangeRoomCommand(
    Guid BookingId, 
    int FromRoomId, 
    int ToRoomId, 
    string? Reason = null) : IRequest<ChangeRoomResult>;

public class ChangeRoomResult
{
    public bool IsSuccess { get; set; }
    public BookingVM? Data { get; set; }
    public double PriceDifference { get; set; }
    public string? Message { get; set; }

    public static ChangeRoomResult Success(BookingVM data, double priceDifference, string message)
    {
        return new ChangeRoomResult
        {
            IsSuccess = true,
            Data = data,
            PriceDifference = priceDifference,
            Message = message
        };
    }

    public static ChangeRoomResult Failure(string message)
    {
        return new ChangeRoomResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
