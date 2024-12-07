using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class SubscriptionPaymentFailedEventHandler(AppDbContext db, BrevoService emailService)
    : INotificationHandler<DomainEventNotification<SubscriptionPaymentFailedEvent>>
{
    public async Task Handle(DomainEventNotification<SubscriptionPaymentFailedEvent> notification, CancellationToken cancellationToken)
    {
        var subscription = await db.UserSubscriptions
            .AsTracking()
            .Include(x => x.Subscription)
            .Include(x => x.User)
                .ThenInclude(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserSubscriptionId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"User subscription is not found by {notification.DomainEvent.UserSubscriptionId}");

        await emailService.SendTransactionalEmailAsync(
                subscription.User.Email,
                subscription.User.Profile.Name,
                BrevoIds.PaymentFailed,
                cancellationToken
        ).ConfigureAwait(false);
    }
}
