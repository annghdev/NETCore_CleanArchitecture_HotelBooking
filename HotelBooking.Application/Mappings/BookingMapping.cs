using AutoMapper;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Bookings.Commands.CreateBooking;
using HotelBooking.Application.Features.Payments;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Mappings;

public class BookingMapping : Profile
{
    public BookingMapping()
    {
        CreateMap<Booking, BookingVM>();
        CreateMap<CreateBookingRequest, BookingVM>();
        CreateMap<CreateBookingRequest, BookingVM>();
        CreateMap<PaymentTransaction, PaymentTransactionVM>();
    }
}
