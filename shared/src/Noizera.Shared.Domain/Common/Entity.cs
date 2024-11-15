namespace Noizera.Common.Domain.Common;

public abstract class Entity : BaseEntity
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private readonly List<DomainEvent> domainEvents = [];

    protected Entity() : base()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = SystemClock.UtcNow;
    }

    public IReadOnlyCollection<DomainEvent> PopDomainEvents()
    {
        List<DomainEvent> copy = [.. domainEvents];
        domainEvents.Clear();

        return copy.AsReadOnly();
    }

    protected void AddDomainEvent(DomainEvent domainEvent) => domainEvents.Add(domainEvent);
}