namespace HotelBooking.Application.Features.Auth.AccessControl.ChangeUserRoles;

public record ChangeUserRolesCommand(Guid UserId, IEnumerable<Guid> RoleIds);
