using AutoMapper;
using HotelBooking.Application.Features.Customers;
using HotelBooking.Application.Features.Customers.Commands.CreateCustomer;
using HotelBooking.Application.Features.Customers.Commands.UpdateCustomer;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class CustomerMapping : Profile
{
    public CustomerMapping()
    {
        CreateMap<Customer, CustomerVM>();
        CreateMap<CreateCustomerCommand, Customer>();
        CreateMap<UpdateCustomerCommand, Customer>();
    }
}
