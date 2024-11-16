using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class PasswordUpdatedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<PasswordUpdatedEvent>>
{
#pragma warning disable IDE0060 // Remove unused parameter
    public void Handle(DomainEventNotification<PasswordUpdatedEvent> notification, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        int a = 1;
        throw new NotImplementedException(db.GetType().Name + a);
    }

    Task INotificationHandler<DomainEventNotification<PasswordUpdatedEvent>>.Handle(DomainEventNotification<PasswordUpdatedEvent> notification, CancellationToken cancellationToken) => throw new NotImplementedException();
}
