using MediatR;

namespace HotelBooking.Application.Features.Auth.AccessControl.CreateRole;

public record CreateRoleCommand(string RoleName, IEnumerable<int> PermissionIds) : IRequest;