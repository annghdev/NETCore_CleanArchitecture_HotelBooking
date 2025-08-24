using HotelBooking.Domain.Entities;
using MediatR;

namespace HotelBooking.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
     string UserName ,
     string? Password ,
     string? Email ,
     string? PhoneNumber ,
     AccountOrigin AccountOrigin): IRequest<UserVM>;
