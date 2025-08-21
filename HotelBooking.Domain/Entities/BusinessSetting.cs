using HotelBooking.Domain.Entities.Bases;

namespace HotelBooking.Domain.Entities;

public class BusinessSetting : AuditableEntity<Guid>
{
    public int PrepayPercent { get; set; } = 20;
    public int PrepayAtLeastDays { get; set; } = 1;
    public bool RequireCheckAllPeople { get; set; }
}

