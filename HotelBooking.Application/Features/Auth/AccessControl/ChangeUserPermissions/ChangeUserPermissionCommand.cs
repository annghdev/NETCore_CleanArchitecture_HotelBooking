using MediatR;

namespace HotelBooking.Application.Features.Auth.AccessControl.ChangeUserPermissions;

public record ChangeUserPermissionCommand(Guid UserId, IEnumerable<int> PermissionIds) : IRequest;
