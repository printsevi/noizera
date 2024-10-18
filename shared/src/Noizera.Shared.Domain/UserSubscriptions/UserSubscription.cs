using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.UserSubscriptions;

public sealed class UserSubscription : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; } = null!;
    public Guid SubscriptionId { get; private set; }
    public Subscription Subscription { get; } = null!;
    public string? SubscriptionStripeId { get; private set; }
    public DateTimeOffset? CurrentPeriodStart { get; private set; }
    public DateTimeOffset? CurrentPeriodEnd { get; private set; }
    public DateTimeOffset? CancelledOn { get; private set; }
    public bool IsActive { get; private set; }
    public string? CheckoutSessionId { get; private set; }
    public DateTimeOffset? CheckoutSessionProcessedOn { get; private set; }

    private UserSubscription(
        User user,
        Subscription subscription)
            : base()
    {
        UserId = user.Id;
        SubscriptionId = subscription.Id;
    }

    public static UserSubscription Create([NotNull] User user, [NotNull] Subscription subscription, string checkoutSessionId)
    {
        UserSubscription result = new(user, subscription);
        result.CreateCheckoutSession(checkoutSessionId);

        return result;
    }

    public void ExpireCheckoutSession() => CheckoutSessionId = null;

    public void CreateCheckoutSession(string sessionId)
    {
        CheckoutSessionId = sessionId;

        AddDomainEvent(new CheckoutSessionCreatedEvent(Id, CheckoutSessionId));
    }

    public void ActivateSubscription(string subscriptionStripeId, DateTimeOffset currentPeriodStart, DateTimeOffset currentPeriodEnd)
    {
        SubscriptionStripeId = subscriptionStripeId;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        CheckoutSessionProcessedOn = SystemClock.UtcNow;
        IsActive = true;

        AddDomainEvent(new UserSubscriptionActivatedEvent(Id));
        AddDomainEvent(new SubscriptionRenewalPlannedEvent(Id, currentPeriodEnd));

        if (Subscription.RoyaltyShare > 0)
        {
            AddDomainEvent(new RoyaltyPaymentPlannedEvent(UserId, currentPeriodStart, currentPeriodEnd, Subscription.RoyaltyShare, Subscription.Price));
        }
    }

    public bool CheckoutSessionIsProcessed => CheckoutSessionProcessedOn.HasValue;

    private UserSubscription() { }
}
