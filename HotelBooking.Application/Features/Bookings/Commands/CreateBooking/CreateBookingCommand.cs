using HotelBooking.Application.Features.Bookings.CalculateAmount;

namespace HotelBooking.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid Id,
    string VoucherCode,
    PaymentOrigin PrepayOrigin) : IRequest<CreateBookingResult>;

public class CreateBookingResult
{
    public bool IsSuccess { get; set; }
    public BookingVM? Data { get; set; }
    public RequirePaymentResult? RequirePayment { get; set; }
    public AmountResult? AmountResult { get; set; }
    public string? Message { get; set; }

    public static CreateBookingResult Success(BookingVM data, AmountResult amountResult, RequirePaymentResult payment)
    {
        return new CreateBookingResult
        {
            IsSuccess = true,
            Data = data,
            RequirePayment = payment,
            AmountResult = amountResult,
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

public record RequirePaymentResult(
    double Amount,
    string? PaymentLink,
    string? Gateway,
    bool IsRedirect,
    int Timeout = 15);

