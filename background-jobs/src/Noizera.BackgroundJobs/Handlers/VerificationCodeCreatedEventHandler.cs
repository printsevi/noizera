using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class VerificationCodeCreatedEventHandler(BrevoService emailService)
    : INotificationHandler<DomainEventNotification<VerificationCodeCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<VerificationCodeCreatedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendConfirmationAccountEmailAsync(
            notification.DomainEvent.Email,
            notification.DomainEvent.Name,
            notification.DomainEvent.Code,
            cancellationToken
        ).ConfigureAwait(false);
}
