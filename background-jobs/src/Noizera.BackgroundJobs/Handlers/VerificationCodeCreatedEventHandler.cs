using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class VerificationCodeCreatedEventHandler(BrevoService emailService)
    : INotificationHandler<DomainEventNotification<VerificationCodeCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<VerificationCodeCreatedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendTransactionalEmailAsync(
            notification.DomainEvent.Email,
            notification.DomainEvent.Name,
            3,
            cancellationToken,
            new Dictionary<string, object>() { { "code", notification.DomainEvent.Code } }
        ).ConfigureAwait(false);
}
