using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Events;

public sealed record VerificationCodeCreatedEvent(string Email, string Code) : DomainEvent;
