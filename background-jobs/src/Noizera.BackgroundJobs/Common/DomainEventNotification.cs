using MediatR;
using Noizera.Shared.Domain.Common;

namespace Noizera.BackgroundJobs.Common;

public class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent)
    : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent => domainEvent;
}
