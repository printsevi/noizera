using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Subscriptions;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class SubscriptionRenewalPlannedEventHandler(AppDbContext db, StripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<SubscriptionRenewalPlannedEvent>>
{
    public async Task Handle(DomainEventNotification<SubscriptionRenewalPlannedEvent> notification, CancellationToken cancellationToken)
    {
        var subscription = await db.UserSubscriptions
            .AsTracking()
            .Include(x => x.Subscription)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserSubscriptionId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"User subscription is not found by {notification.DomainEvent.UserSubscriptionId}");

        var stripeSubscription = await subscriptionService.GetSubscriptionAsync(subscription.SubscriptionStripeId!, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Stripe subscription is not found by {subscription.SubscriptionStripeId}");

        switch (stripeSubscription.Status)
        {
            case "past_due":
                subscription.DeclareFailedPayment(SystemClock.UtcNow.AddDays(1));
                break;
            case "canceled" or "unpaid":
                subscription.CancelSubscription();
                break;
            case "active":
                if (stripeSubscription.CurrentPeriodEnd <= subscription.CurrentPeriodEnd!.Value.AddDays(2).DateTime)
                {
                    throw new RetryException(delayInSeconds: 24 * 60 * 60);
                }

                subscription.RenewSubscription(stripeSubscription.CurrentPeriodStart, stripeSubscription.CurrentPeriodEnd);
                break;
            default:
                throw new InvalidOperationException($"Unknown stripe status: '{stripeSubscription.Status}'. Unable to process.");
        }
    }
}
