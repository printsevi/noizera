using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;

namespace Noizera.BackgroundJobs.Handlers;

internal class VerificationCodeCreatedEventHandler(EmailService emailService)
    : INotificationHandler<DomainEventNotification<VerificationCodeCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<VerificationCodeCreatedEvent> notification, CancellationToken cancellationToken)
        => await emailService.SendEmailAsync(
            EmailTemplateNames.AccountConfirmation,
            "ildarprintsev@gmail.com",//notification.DomainEvent.Email,
            "ildarprintsev@gmail.com",//notification.DomainEvent.Email,
            "notifications@noizera.com",
            "Noizera Notifications",
            "Confirm email",
            cancellationToken,
            new Dictionary<string, string>() { { "//p[@id='verification-code']", notification.DomainEvent.Code } }
        ).ConfigureAwait(false);
}
