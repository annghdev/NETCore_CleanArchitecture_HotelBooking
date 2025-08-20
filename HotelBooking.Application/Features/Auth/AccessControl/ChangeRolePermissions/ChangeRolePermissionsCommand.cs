using MediatR;

namespace HotelBooking.Application.Features.Auth.AccessControl.ChangeRolePermissions;

public record ChangeRolePermissionsCommand(Guid RoleId, IEnumerable<int> PermissionIds) : IRequest;