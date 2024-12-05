using Microsoft.Extensions.Configuration;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.UserSubscriptions;
using Noizera.Common.Persistence.SQL;
using Stripe;
using System.Diagnostics.CodeAnalysis;
using Subscription = Noizera.Common.Domain.Subscriptions.Subscription;

namespace Noizera.Common.Infrastructure.Subscriptions;

public class SubscriptionStripeService(AppDbContext db, StripeService stripeService, IConfiguration configuration)
{
    private readonly string host = configuration.GetSection("FrontendUrls")?.Get<string[]>()?.FirstOrDefault() ?? string.Empty;

    public async Task CreateStripeCustomerAsync([NotNull] User user, CancellationToken ct)
    {
        string? customerId = await stripeService.CreateCustomerAsync(user.Email, user.Profile!.Name, ct).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new StripeException($"Failed to create a Stripe customer for user {user.Id}");
        }

        user.SetCustomerStripeId(customerId);
    }

    public async Task<Uri> CreateCheckoutSessionAsync([NotNull] User user, [NotNull] Subscription subscription, CancellationToken cancellationToken)
    {
        var incompleteUserSubscription = user.GetIncompleteSubscriptionOfType(subscription.SubscriptionType);
        if (incompleteUserSubscription?.CheckoutSessionId is not null)
        {
            var session = await stripeService.GetSessionAsync(incompleteUserSubscription.CheckoutSessionId, cancellationToken).ConfigureAwait(false);
            if (session?.IsPaid ?? false)
            {
                throw new InvalidOperationException($"The subscription {subscription.SubscriptionType} is already paid. Please, wait a bit for the activation.");
            }

            await stripeService.ExpireSessionAsync(incompleteUserSubscription.CheckoutSessionId, cancellationToken).ConfigureAwait(false);
            incompleteUserSubscription.ExpireCheckoutSession();
        }

        short? trialPeriodDays = user.GetTrialDaysIfEntitled(subscription);

        var checkoutSession = await stripeService.CreateCheckoutSessionAsync(
            subscription.StripePriceId,
            user.CustomerStripeId!,
            new Uri($"{host}/system/success?session_id={{CHECKOUT_SESSION_ID}}"),
            new Uri(host),
            trialPeriodDays,
            cancellationToken).ConfigureAwait(false);

        if (checkoutSession is null
            || string.IsNullOrWhiteSpace(checkoutSession.Value.SessionId)
            || checkoutSession.Value.Url is null)
        {
            throw new StripeException($"Failed to create a checkout session for user {user.Id}");
        }

        if (incompleteUserSubscription is null)
        {
            incompleteUserSubscription = UserSubscription.Create(user, subscription, checkoutSession.Value.SessionId);
            _ = await db.UserSubscriptions.AddAsync(incompleteUserSubscription, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            incompleteUserSubscription.CreateCheckoutSession(checkoutSession.Value.SessionId);
            _ = db.UserSubscriptions.Update(incompleteUserSubscription);
        }

        _ = await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return checkoutSession.Value.Url;
    }

    public async Task TryActivateSubscriptionAsync(string checkoutSessionId, [NotNull] UserSubscription userSubscription, CancellationToken ct)
    {
        var session = await stripeService.GetSessionAsync(checkoutSessionId, ct).ConfigureAwait(false);
        if (session is null
            || !session.IsPaid
            || string.IsNullOrWhiteSpace(session.SubscriptionId)
            || !session.CurrentPeriodStart.HasValue
            || !session.CurrentPeriodEnd.HasValue)
        {
            throw new StripeException($"The session {checkoutSessionId} is not found or the invoice is unpaid");
        }

        userSubscription.ActivateSubscription(session.SubscriptionId, session.CurrentPeriodStart.Value, session.CurrentPeriodEnd.Value, session.IsTrial);
    }

    public async Task<Uri?> GetSubscriptionPortalUrlAsync([NotNull] User user, CancellationToken ct)
    {
        var result = await stripeService.GetBillingPortalLinkAsync(user.CustomerStripeId!, new Uri(host), ct).ConfigureAwait(false);

        return result;
    }

    public async Task<string?> GetCheckoutSessionStatus(string sessionId, CancellationToken ct = default)
        => await stripeService.GetCheckoutSessionStatusAsync(sessionId, ct).ConfigureAwait(false);
}
