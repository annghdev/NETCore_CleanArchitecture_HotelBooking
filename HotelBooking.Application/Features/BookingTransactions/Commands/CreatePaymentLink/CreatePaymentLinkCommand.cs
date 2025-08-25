using HotelBooking.Application.Features.Auth.Login;
using HotelBooking.Application.Services.Payments;

namespace HotelBooking.Application.Features.BookingTransactions.Commands.CreatePaymentLink;

public record CreatePaymentLinkCommand(
    Guid BookingId,
    PaymentAction Action,
    PaymentGateway Gateway,
    OpenPlatform OpenPlatform,
    double Amount,
    string Information) : IRequest<PaymentLinkResponse>;

