using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal class ResetTokenCreatedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<ResetTokenCreatedEvent>>
{
#pragma warning disable IDE0060 // Remove unused parameter
    public void Handle(DomainEventNotification<ResetTokenCreatedEvent> notification, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        string a = db.GetType().ToString();
        throw new NotImplementedException(a);
    }

    Task INotificationHandler<DomainEventNotification<ResetTokenCreatedEvent>>.Handle(DomainEventNotification<ResetTokenCreatedEvent> notification, CancellationToken cancellationToken) => throw new NotImplementedException();
}
