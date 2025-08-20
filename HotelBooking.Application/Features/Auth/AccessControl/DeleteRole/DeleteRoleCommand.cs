using MediatR;

namespace HotelBooking.Application.Features.Auth.AccessControl.DeleteRole;

public record DeleteRoleCommand(Guid RoleId): IRequest;
