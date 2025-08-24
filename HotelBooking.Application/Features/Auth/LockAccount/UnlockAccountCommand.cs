using MediatR;

namespace HotelBooking.Application.Features.Auth.LockAccount;

public record UnlockAccountCommand(Guid UserId): IRequest;
