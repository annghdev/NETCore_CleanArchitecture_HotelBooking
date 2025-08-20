namespace HotelBooking.Domain.Entities.Bases
{
    public interface ISoftDeletable
    {
        DateTimeOffset? DeletedDate { get; set; }
    }
}
