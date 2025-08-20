namespace HotelBooking.Domain.Entities.Bases
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
