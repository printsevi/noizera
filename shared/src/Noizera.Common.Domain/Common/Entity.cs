using System.Globalization;

namespace Noizera.Common.Domain.Common;

public abstract class Entity : BaseEntity
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt => GetCreatedDateFromGuid(Id);

    private readonly List<DomainEvent> domainEvents = [];

    protected Entity() : base()
        => Id = Guid.CreateVersion7();

    public IReadOnlyCollection<DomainEvent> PopDomainEvents()
    {
        List<DomainEvent> copy = [.. domainEvents];
        domainEvents.Clear();

        return copy.AsReadOnly();
    }

    protected void AddDomainEvent(DomainEvent domainEvent) => domainEvents.Add(domainEvent);

    private static DateTimeOffset GetCreatedDateFromGuid(Guid guid)
    {
        string[] parts = guid.ToString().Split('-');

        // Combine the first part and the first 4 characters of the second part to get the high bits
        string highBitsHex = string.Concat(parts[0], parts[1].AsSpan(0, 4));

        // Convert the high bits from hex to decimal
        long timestampInMilliseconds = long.Parse(highBitsHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

        // Convert the timestamp to a DateTimeOffset object (milliseconds since Unix epoch)
        return DateTimeOffset.FromUnixTimeMilliseconds(timestampInMilliseconds);
    }
}