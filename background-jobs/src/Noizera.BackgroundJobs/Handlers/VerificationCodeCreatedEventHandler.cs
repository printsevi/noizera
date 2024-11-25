using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class VerificationCodeCreatedEventHandler(EmailService emailService)
    : INotificationHandler<DomainEventNotification<VerificationCodeCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<VerificationCodeCreatedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendEmailAsync(
            EmailTemplateNames.AccountConfirmation,
            notification.DomainEvent.Email,
            notification.DomainEvent.Email,
            "notifications@noizera.com",
            "Noizera",
            "Confirm email",
            cancellationToken,
            new Dictionary<string, string>() { { "//p[@id='verification-code']", notification.DomainEvent.Code } }
        ).ConfigureAwait(false);
}
