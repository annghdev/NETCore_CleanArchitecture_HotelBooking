using MediatR;

namespace HotelBooking.Application.Features.Auth.RegisterAccount;

public record RegisterAccountCommand(
    string UserName,
    string Password,
    string? Email,
    string? PhoneNumber,
    bool IsNewCustomer,
    Guid? CustomerId,
    string? FullName) : IRequest<AuthResponse>;