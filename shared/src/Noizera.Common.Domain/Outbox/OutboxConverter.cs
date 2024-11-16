using Noizera.Common.Domain.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Noizera.Common.Domain.Outbox;

public static class OutboxConverter
{
    public static OutboxMessage ConvertToOutboxMessage([NotNull] DomainEvent domainEvent)
    {
        string typeName = domainEvent.GetType().Name!;
        string data = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());
        var outboxMessage = OutboxMessage.New(domainEvent.OccurredOn, typeName, data, domainEvent.RealTime, domainEvent.ProcessAfter);

        return outboxMessage;
    }

    public static DomainEvent ConvertToDomainEvent([NotNull] OutboxMessage message)
    {
        var eventType = Type.GetType($"Noizera.Common.Domain.Events.{message.Type}")
            ?? throw new JsonException($"{message.Type} is unknown");
        object domainEvent = JsonSerializer.Deserialize(message.Data, eventType)
            ?? throw new JsonException($"{message.Data} cannot be deserialized to {eventType.FullName}");

        return (DomainEvent)domainEvent;
    }
}
