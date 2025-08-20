using MediatR;

namespace HotelBooking.Application.Features.Auth.ConfirmEmail;

public record ConfirmAccountCommand(Guid UserId, ConfirmType ConfirmType, string Credential) : IRequest;

public enum ConfirmType
{
    ByEmail,
    ByPhoneNumber
}