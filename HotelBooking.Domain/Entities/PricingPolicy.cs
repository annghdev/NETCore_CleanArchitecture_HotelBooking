using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class PricingPolicy : AuditableEntity<int>, IAggregateRoot
{
    public string Name { get; set; } = default!;
    public RoomType RoomType { get; set; }
    public BookingType PricingType { get; set; }
    public double BasePrice { get; set; }
    public double ExtraUnitPrice { get; set; } // phụ thu

    //Cho đặt theo giờ và qua đêm
    public int? MinDuration { get; set; } // giờ
    public int? MaxDuration { get; set; }
    public string? DaysOfWeek { get; set; }

    //Cho đặt theo ngày và qua đêm
    public TimeOnly? CheckInTimeDefault { get; set; }
    public TimeOnly? CheckOutTimeDefault { get; set; }

}
