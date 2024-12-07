using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class ContactFormSubmittedEventHandler(BrevoService emailService)
    : INotificationHandler<DomainEventNotification<ContactFormSubmittedEvent>>
{
    public async Task Handle(DomainEventNotification<ContactFormSubmittedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendTransactionalEmailAsync(
            notification.DomainEvent.Email,
            notification.DomainEvent.Name,
            BrevoIds.ContactFormSubmitted,
            cancellationToken,
            new Dictionary<string, object>() { { "description", notification.DomainEvent.Description } },
            [("help@noizera.com", "Noizera")],
            $"📝 Form Submission Received: {notification.DomainEvent.Topic}"
        ).ConfigureAwait(false);
}
