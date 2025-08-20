using MediatR;

namespace HotelBooking.Application.Features.Auth.LockUser;

public record LockAccountCommand(Guid UserId, DateTimeOffset? LockTo = null) : IRequest;