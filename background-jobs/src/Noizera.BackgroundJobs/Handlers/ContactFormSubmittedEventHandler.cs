using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class ContactFormSubmittedEventHandler(BrevoService emailService)
    : INotificationHandler<DomainEventNotification<ContactFormSubmittedEvent>>
{
    public async Task Handle(DomainEventNotification<ContactFormSubmittedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendContactFormAsync(
            notification.DomainEvent.Email,
            notification.DomainEvent.Name,
            notification.DomainEvent.Topic,
            notification.DomainEvent.Description,
            cancellationToken
        ).ConfigureAwait(false);
}
