using AutoMapper;
using HotelBooking.Application.Common;
using HotelBooking.Application.Features.Customers;
using HotelBooking.Application.Features.Customers.Commands.CreateCustomer;
using HotelBooking.Application.Features.Customers.Commands.UpdateCustomer;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class CustomerMapping : Profile
{
    public CustomerMapping()
    {
        // Customer Entity to CustomerVM - Convert UTC to Vietnam timezone
        CreateMap<Customer, CustomerVM>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CreatedDate)));

        // Commands to Entity - No timezone conversion needed (handled by Entity base classes)
        CreateMap<CreateCustomerCommand, Customer>();
        CreateMap<UpdateCustomerCommand, Customer>();
    }
}
