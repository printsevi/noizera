using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

public class ResetTokenCreatedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<ResetTokenCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<ResetTokenCreatedEvent> notification, CancellationToken cancellationToken)
    {
        
    }
}
