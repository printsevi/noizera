using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.UserSubscriptions;

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

    public void ActivateSubscription(string subscriptionStripeId, DateTimeOffset currentPeriodStart, DateTimeOffset currentPeriodEnd, bool isTrial)
    {
        if (IsActive)
        {
            return;
        }

        SubscriptionStripeId = subscriptionStripeId;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        CheckoutSessionProcessedOn = SystemClock.UtcNow;
        IsActive = true;

        AddDomainEvent(new UserSubscriptionActivatedEvent(Id));
        AddDomainEvent(new SubscriptionRenewalPlannedEvent(Id, currentPeriodEnd));

        if (Subscription.RoyaltyShare > 0 && !isTrial)
        {
            AddDomainEvent(new RoyaltyPaymentPlannedEvent(UserId, currentPeriodStart, currentPeriodEnd, Subscription.RoyaltyShare, Subscription.Price));
        }
    }

    public void DeclareFailedPayment(DateTimeOffset nextCheck)
    {
        AddDomainEvent(new SubscriptionPaymentFailedEvent(Id));
        AddDomainEvent(new SubscriptionRenewalPlannedEvent(Id, nextCheck));
    }

    public void CancelSubscription()
    {
        CancelledOn = SystemClock.UtcNow;
        IsActive = false;
        AddDomainEvent(new SubscriptionCancelledEvent(Id));
    }

    public void RenewSubscription(DateTimeOffset currentPeriodStart, DateTimeOffset currentPeriodEnd)
    {
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        IsActive = true;

        AddDomainEvent(new SubscriptionRenewalPlannedEvent(Id, currentPeriodEnd));

        if (Subscription.RoyaltyShare > 0)
        {
            AddDomainEvent(new RoyaltyPaymentPlannedEvent(UserId, currentPeriodStart, currentPeriodEnd, Subscription.RoyaltyShare, Subscription.Price));
        }
    }

    public bool CheckoutSessionIsProcessed => CheckoutSessionProcessedOn.HasValue;

    private UserSubscription() { }
}
