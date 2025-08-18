namespace HotelBooking.Domain.Abstractions;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
