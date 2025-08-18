namespace HotelBooking.Domain.Abstractions;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedDate { get; set; }
}
