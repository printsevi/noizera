using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Events;

public sealed record ContactFormSubmittedEvent(string Email, string Name, string Topic, string Description) : DomainEvent;
