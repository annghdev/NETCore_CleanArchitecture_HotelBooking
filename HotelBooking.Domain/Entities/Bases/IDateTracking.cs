namespace HotelBooking.Domain.Entities.Bases;

public interface IDateTracking
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset? UpdatedDate { get; set; }
}
