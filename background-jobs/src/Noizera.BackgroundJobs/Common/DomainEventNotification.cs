using MediatR;
using Noizera.Common.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.BackgroundJobs.Common;

internal sealed class DomainEventNotification<TDomainEvent>([NotNull] TDomainEvent domainEvent)
    : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent => domainEvent;
}
