using HotelBooking.Domain.Abstractions;

namespace HotelBooking.Domain.Entities;

public class TimeFrame : EntityBase<int>, IAggregateRoot
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double BasePrice { get; set; }
}
