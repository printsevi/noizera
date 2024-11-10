using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

public class PasswordUpdatedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<PasswordUpdatedEvent>>
{
    public void Handle(DomainEventNotification<PasswordUpdatedEvent> notification, CancellationToken cancellationToken)
    {

    }

    Task INotificationHandler<DomainEventNotification<PasswordUpdatedEvent>>.Handle(DomainEventNotification<PasswordUpdatedEvent> notification, CancellationToken cancellationToken) => throw new NotImplementedException();
}
