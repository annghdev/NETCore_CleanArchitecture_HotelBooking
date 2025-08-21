using HotelBooking.Application.Features.Bookings.Commands.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingResult
{
    public bool IsSuccess { get; set; }
    public BookingVM? Data { get; set; }
    public string? PaymentLink { get; set; }
    public string? Message { get; set; }
    public CalculatedAmountResult? EstimateAmount { get; set; }

    public static CreateBookingResult Success(BookingVM data, string? paymentLink)
    {
        return new CreateBookingResult
        {
            IsSuccess = true,
            Data = data,
            PaymentLink = paymentLink
        };
    }
    public static CreateBookingResult Failure(string message)
    {
        return new CreateBookingResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}
