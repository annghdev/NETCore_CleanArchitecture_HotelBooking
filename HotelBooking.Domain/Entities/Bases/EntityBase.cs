using MediatR;

namespace HotelBooking.Domain.Entities.Bases;

public abstract class EntityBase<TKey> : IEntity<TKey>
{
    public TKey Id { get; set; } = default!;

    private readonly List<DomainEventBase> _events = new List<DomainEventBase>();

    public IReadOnlyList<DomainEventBase> DomainEvents => _events;

    public void AddDomainEvent(DomainEventBase domainEvent)
    {
        _events.Add(domainEvent);
    }
}

public record DomainEventBase : INotification
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTimeOffset OccuredDate { get; init; } = DateTimeOffset.UtcNow;
}