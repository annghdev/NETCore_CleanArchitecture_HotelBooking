using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.DraftBooking;

public record DraftBookingCommand(
    Guid? CustomerId,
    IEnumerable<int> RoomIds,
    BookingType Type,
    BookingOrigin Origin,
    DateTimeOffset CheckInDateTime,
    DateTimeOffset CheckOutDateTime,
    string? SessionId) : IRequest<DraftBookingResult>;

public class DraftBookingResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public BookingVM? BookingInfo { get; set; }
    public AmountResult? AmountResult { get; set; }
    public DateTimeOffset? ExpiryTime { get; set; }

    public static DraftBookingResult Success(AmountResult amountResult, BookingVM bookingInfo, DateTimeOffset expiryTime)
    {
        return new DraftBookingResult
        {
            IsSuccess = true,
            AmountResult = amountResult,
            BookingInfo = bookingInfo,
            ExpiryTime = expiryTime
        };
    }

    public static DraftBookingResult Failure(string message)
    {
        return new DraftBookingResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
