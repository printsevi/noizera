using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Infrastructure.Emails;
using Noizera.Shared.Infrastructure.Subscriptions;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

public class UserSubscriptionActivatedEventHandler(
    AppDbContext db,
    EmailService emailService)
    : INotificationHandler<DomainEventNotification<UserSubscriptionActivatedEvent>>
{
    public async Task Handle(DomainEventNotification<UserSubscriptionActivatedEvent> notification, CancellationToken cancellationToken)
    {
        var userSubscription = await db.UserSubscriptions
            .Include(x => x.User)
            .Include(x => x.Subscription)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserSubscriptionId, cancellationToken)
            ?? throw new Exception($"UserSubscription {notification.DomainEvent.UserSubscriptionId} is not found.");

        await emailService.SendEmailAsync(
            EmailTemplateNames.AccountConfirmation,
            userSubscription.User.Email,
            userSubscription.User.Email,
            "notifications@noizera.com",
            "Noizera Notifications",
            "Your subscription is activated",
            new Dictionary<string, string>() { { "verification-code", notification.DomainEvent.UserSubscriptionId.ToString() } }
        );
    }
}
