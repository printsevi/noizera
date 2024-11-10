using Microsoft.Extensions.Configuration;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Domain.UserSubscriptions;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Shared.Infrastructure.Subscriptions;

public class SubscriptionStripeService(AppDbContext db, StripeService stripeService)
{
    public async Task CreateStripeCustomerAsync(User user, CancellationToken ct)
    {
        string? customerId = await stripeService.CreateCustomerAsync(user.Id, user.Email, user.Profile!.Name, ct).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new Exception($"Failed to create a Stripe customer");
        }

        user.SetCustomerStripeId(customerId);
    }

    public async Task<Uri> CreateCheckoutSessionAsync(User user, Subscription subscription, IConfiguration configuration, CancellationToken cancellationToken)
    {
        var incompleteUserSubscription = user.GetIncompleteSubscriptionOfType(subscription.SubscriptionType);
        if (incompleteUserSubscription is not null)
        {
            bool isExpired = await stripeService.ExpireSessionAsync(incompleteUserSubscription.CheckoutSessionId!, cancellationToken).ConfigureAwait(false);
            incompleteUserSubscription.ExpireCheckoutSession();
        }

        string? frontendUrl = configuration["FrontendUrl"];

        var trialPeriodDays = user.GetTrialDaysIfEntitled(subscription.SubscriptionType);

        var checkoutSession = await stripeService.CreateCheckoutSessionAsync(
            subscription.StripePriceId,
            user.CustomerStripeId!,
            new Uri($"{frontendUrl}/system/success?session_id={{CHECKOUT_SESSION_ID}}"),
            new Uri(frontendUrl!),
            trialPeriodDays,
            cancellationToken).ConfigureAwait(false);

        if (checkoutSession is null
            || string.IsNullOrWhiteSpace(checkoutSession.Value.SessionId)
            || checkoutSession.Value.Url is null)
        {
            throw new Exception($"Failed to create a checkout session");
        }

        if (incompleteUserSubscription is null)
        {
            incompleteUserSubscription = UserSubscription.Create(user, subscription, checkoutSession.Value.SessionId);
            await db.UserSubscriptions.AddAsync(incompleteUserSubscription, cancellationToken);
        }
        else
        {
            incompleteUserSubscription.CreateCheckoutSession(checkoutSession.Value.SessionId);
            db.UserSubscriptions.Update(incompleteUserSubscription);
        }

        await db.SaveChangesAsync(cancellationToken);

        return checkoutSession.Value.Url;
    }

    public async Task ActivateSubscriptionAsync(string checkoutSessionId, UserSubscription userSubscription, CancellationToken ct)
    {
        var session = await stripeService.GetSessionAsync(checkoutSessionId, ct).ConfigureAwait(false);
        if (session is null
            || !session.IsPaid
            || string.IsNullOrWhiteSpace(session.SubscriptionId)
            || !session.CurrentPeriodStart.HasValue
            || !session.CurrentPeriodEnd.HasValue)
        {
            throw new Exception($"The session is not found or the invoice is unpaid");
        }

        userSubscription.ActivateSubscription(session.SubscriptionId, session.CurrentPeriodStart.Value, session.CurrentPeriodEnd.Value);
    }

    public async Task<Uri?> GetSubscriptionPortalUrlAsync(User user, IConfiguration configuration, CancellationToken ct)
    {
        var result = await stripeService.GetBillingPortalLinkAsync(user.CustomerStripeId!, new Uri(configuration["FrontendUrl"]!), ct).ConfigureAwait(false);

        return result;
    }

    public async Task<string?> GetCheckoutSessionStatus(string sessionId, CancellationToken ct = default)
    {
        return await stripeService.GetCheckoutSessionStatusAsync(sessionId, ct).ConfigureAwait(false);
    }
}
