using AutoMapper;
using HotelBooking.Application.Features.Bookings;
using HotelBooking.Application.Features.Payments;

namespace HotelBooking.Application.Mappings;

public class BookingMapping : Profile
{
    public BookingMapping()
    {
        // Booking Entity to BookingVM - Convert UTC to Vietnam timezone
        CreateMap<Booking, BookingVM>()
            .ForMember(dest => dest.CheckInDateTime, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CheckInDateTime)))
            .ForMember(dest => dest.CheckOutDateTime, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CheckOutDateTime)))
            .ForMember(dest => dest.CheckedInAt, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CheckedInAt)))
            .ForMember(dest => dest.CheckedOutAt, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.CheckedOutAt)))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.OriginName, opt => opt.MapFrom(src => src.Origin.ToString()))
            .ForMember(dest => dest.PaymentStatusName, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        // BookingRoom Entity to BookingRoomVM - Convert UTC to Vietnam timezone
        CreateMap<BookingRoom, BookingRoomVM>()
            .ForMember(dest => dest.ChangedRoomDate, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.ChangedRoomDate)));

        // PaymentTransaction Entity to PaymentTransactionVM - Convert UTC to Vietnam timezone
        CreateMap<PaymentTransaction, PaymentTransactionVM>()
            .ForMember(dest => dest.OccuredDate, opt => opt.MapFrom(src => TimeZoneHelper.ConvertUtcOffsetToVietnamOffset(src.OccuredDate)))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.OriginName, opt => opt.MapFrom(src => src.Origin.ToString()))
            .ForMember(dest => dest.ProcessStatusName, opt => opt.MapFrom(src => src.ProcessStatus.ToString()));

    }
}
