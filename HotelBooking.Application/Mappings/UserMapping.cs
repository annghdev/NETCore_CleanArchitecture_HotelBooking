using AutoMapper;
using HotelBooking.Application.Features.Users;
using HotelBooking.Application.Features.Users.Commands.CreateUser;
using HotelBooking.Application.Features.Users.Commands.UpdateUser;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserVM>();
        CreateMap<CreateUserCommand, UserVM>();
        CreateMap<UpdateUserCommand, UserVM>();
    }
}
