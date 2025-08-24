using AutoMapper;
using HotelBooking.Application.Common.Helpers;
using HotelBooking.Application.Features.Users;
using HotelBooking.Application.Features.Users.Commands.CreateUser;
using HotelBooking.Application.Features.Users.Commands.UpdateUser;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class UserMapping : Profile
{
    public UserMapping()
    {
        // User to UserVM mapping
        CreateMap<User, UserVM>()
            .ForMember(dest => dest.AccountOrigin, opt => opt.MapFrom(src => src.AccountOrigin));

        // Command to UserVM mappings (for consistency)
        CreateMap<CreateUserCommand, UserVM>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateUserCommand, UserVM>();

        // User to UserProfileVM mapping - Convert UTC to Vietnam timezone
        CreateMap<User, UserProfileVM>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AccountOriginName, opt => opt.MapFrom(src => src.AccountOrigin.ToString()))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CreatedDate)));

        // Customer to CustomerProfileVM mapping
        CreateMap<Customer, UserProfileVM>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
    }
}
