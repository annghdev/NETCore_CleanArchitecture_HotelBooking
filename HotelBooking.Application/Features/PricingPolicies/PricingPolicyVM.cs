using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Features.PricingPolicies;

public class PricingPolicyVM
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RoomType RoomType { get; set; }
    public RoomType RoomTypeName { get; set; }
    public BookingType PricingType { get; set; }
    public BookingType PricingTypeName { get; set; }
    public double BasePrice { get; set; }
    public double ExtraUnitPrice { get; set; } // phụ thu

    //Cho đặt theo giờ và qua đêm
    public int? MinDuration { get; set; } // giờ
    public int? MaxDuration { get; set; }

    //Cho đặt theo ngày và qua đêm
    public TimeOnly? CheckInTimeDefault { get; set; }
    public TimeOnly? CheckOutTimeDefault { get; set; }
}
