using HotelBooking.Application.Features.Auth.Login;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.Application.Services.Payments;

public interface IPaymentService
{
    Task<PaymentLinkResponse> CreatePaymentLinkAsync(PaymentRequest request);
    Task<PaymenVerificationtResponse> VerifyPaymentAsync(PaymentVerificationRequest request);
}

public class PaymentLinkResponse
{
    public bool IsSuccess { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public bool IsRedirect { get; set; }
    public OpenPlatform OpenPlatform { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class PaymentRequest
{
    public string FullName { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string OrderInfo { get; set; } = string.Empty;
    public double Amount { get; set; }
    public OpenPlatform Platform { get; set; } // "mobile" hoặc "web"
    public HttpContext? Context { get; set; } // Dùng cho VnPay hoặc các cổng cần IP
}

public class PaymentResult
{
    public string OrderId { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
}

public class PaymenVerificationtResponse
{
    public bool IsSuccess { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string? OrderInfo { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public Guid? BookingId { get; set; }
}

public class PaymentVerificationRequest
{
    public IDictionary<string, string> IpnData { get; set; } // Dữ liệu từ IPN
    public IQueryCollection QueryData { get; set; } // Dữ liệu từ callback
}
