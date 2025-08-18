namespace HotelBooking.Domain.Abstractions;

public abstract class EntityBase<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}
