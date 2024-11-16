using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class UserSubscriptionActivatedEventHandler(
    AppDbContext db,
    EmailService emailService)
    : INotificationHandler<DomainEventNotification<UserSubscriptionActivatedEvent>>
{
    public async Task Handle(DomainEventNotification<UserSubscriptionActivatedEvent> notification, CancellationToken cancellationToken)
    {
        var userSubscription = await db.UserSubscriptions
            .Include(x => x.User)
            .Include(x => x.Subscription)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserSubscriptionId, cancellationToken).ConfigureAwait(false)
            ?? throw new ArgumentException($"UserSubscription {notification.DomainEvent.UserSubscriptionId} is not found.");

        await emailService.SendEmailAsync(
            EmailTemplateNames.AccountConfirmation,
            userSubscription.User.Email,
            userSubscription.User.Email,
            "notifications@noizera.com",
            "Noizera Notifications",
            "Your subscription is activated",
            cancellationToken,
            new Dictionary<string, string>() { { "verification-code", notification.DomainEvent.UserSubscriptionId.ToString() } }
        ).ConfigureAwait(false);
    }
}
