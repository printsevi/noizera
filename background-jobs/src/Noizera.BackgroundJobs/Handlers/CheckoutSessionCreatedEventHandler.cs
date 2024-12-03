using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Subscriptions;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class CheckoutSessionCreatedEventHandler(AppDbContext db, SubscriptionStripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<CheckoutSessionCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<CheckoutSessionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var subscription = await db.UserSubscriptions
            .AsTracking()
            .Include(x => x.Subscription)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserSubscriptionId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"User subscription is not found by {notification.DomainEvent.UserSubscriptionId}");

        if (subscription.CheckoutSessionIsProcessed || subscription.CheckoutSessionId is null || subscription.CheckoutSessionId != notification.DomainEvent.StripeSessionId)
        {
            return;
        }

        string? sessionStatus = await subscriptionService.GetCheckoutSessionStatus(subscription.CheckoutSessionId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Stripe session is not found by {subscription.CheckoutSessionId}");

        switch (sessionStatus)
        {
            case "complete":
                await subscriptionService.ActivateSubscriptionAsync(subscription.CheckoutSessionId, subscription, cancellationToken).ConfigureAwait(false);
                break;
            case "open":
                throw new RetryException(delayInSeconds: 10);
            case "expired":
            default:
                return;
        }
    }
}