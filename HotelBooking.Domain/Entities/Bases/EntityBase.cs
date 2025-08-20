namespace HotelBooking.Domain.Entities.Bases;

public abstract class EntityBase<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}
