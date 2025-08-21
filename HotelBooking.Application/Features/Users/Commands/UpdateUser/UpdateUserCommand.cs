using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
     Guid Id,
     string UserName,
     string? Password,
     string? Email,
     string? Phone,
     AccountOrigin? AccountOrigin) : IRequest;
