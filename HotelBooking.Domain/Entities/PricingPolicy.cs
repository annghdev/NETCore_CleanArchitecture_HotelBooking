using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class PricingPolicy : AuditableEntity<int>
{
    public RoomType RoomType { get; set; }
    public BookingType PricingType { get; set; }
    public double BasePrice { get; set; }
    public double ExtraUnitPrice { get; set; } // phụ thu

    //Cho đặt theo giờ và qua đêm
    public int? MinDuration { get; set; } // giờ
    public int? MaxDuration { get; set; }

    //Cho đặt theo ngày và qua đêm
    public TimeOnly? CheckInTimeDefault { get; set; }
    public TimeOnly? CheckOutTimeDefault { get; set; }

}
