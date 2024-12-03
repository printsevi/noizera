using Stripe;
using Stripe.Checkout;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Infrastructure.Subscriptions;

public class StripeService(
    SessionService sessionService,
    CustomerService customerService,
    Stripe.SubscriptionService subscriptionService,
    Stripe.BillingPortal.SessionService billingSessionService)
{
    public async Task<(Uri Url, string SessionId)?> CreateCheckoutSessionAsync(
        string priceId,
        string customerId,
        [NotNull] Uri SuccessUrl,
        [NotNull] Uri CancelUrl,
        int? trialPeriodDays = null,
        CancellationToken ct = default)
    {
        SessionCreateOptions options = new()
        {
            SuccessUrl = SuccessUrl.ToString(),
            CancelUrl = CancelUrl.ToString(),
            Customer = customerId,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1,
                },
            ],
            Mode = "subscription",
            SubscriptionData = new SessionSubscriptionDataOptions()
        };

        if (trialPeriodDays.HasValue && trialPeriodDays > 0)
        {
            options.SubscriptionData.TrialPeriodDays = trialPeriodDays;
        }

        var session = await sessionService.CreateAsync(options, cancellationToken: ct).ConfigureAwait(false);

        return session is not null ? (new Uri(session.Url), session.Id) : null;
    }

    public async Task<string?> CreateCustomerAsync(
        string userEmail,
        string userName,
        CancellationToken ct)
    {
        CustomerCreateOptions options = new()
        {
            Name = userName,
            Email = userEmail,
        };

        var customer = await customerService.CreateAsync(options, cancellationToken: ct).ConfigureAwait(false);

        return customer?.Id;
    }

    public async Task<StripeSession?> GetSessionAsync(string sessionId, CancellationToken ct)
    {
        var session = await sessionService.GetAsync(sessionId, cancellationToken: ct).ConfigureAwait(false);
        if (session is null)
        {
            return null;
        }

        Stripe.Subscription? subscription = null;
        if (!string.IsNullOrEmpty(session.SubscriptionId))
        {
            subscription = await subscriptionService.GetAsync(session.SubscriptionId, cancellationToken: ct).ConfigureAwait(false);
        }

        string? subscriptionStatus = subscription?.Status;
        bool isTrial = subscriptionStatus == "trialing";
        bool isPaid = session.Status == "complete"
            && session.PaymentStatus == "paid"
            && (subscriptionStatus == "active"
                || subscriptionStatus == "trialing");

        return new(
            isPaid,
            isTrial,
            session.SubscriptionId,
            subscription?.CurrentPeriodStart,
            subscription?.CurrentPeriodEnd);
    }

    public async Task<string?> GetCheckoutSessionStatusAsync(string sessionId, CancellationToken ct)
    {
        var session = await sessionService.GetAsync(sessionId, cancellationToken: ct).ConfigureAwait(false);

        return session?.Status;
    }

    public async Task ExpireSessionAsync(string sessionId, CancellationToken ct)
    {
        var session = await sessionService.GetAsync(sessionId, cancellationToken: ct).ConfigureAwait(false);
        if (session?.Status == "open")
        {
            _ = await sessionService.ExpireAsync(sessionId, cancellationToken: ct).ConfigureAwait(false);
        }
    }

    public async Task<Uri?> GetBillingPortalLinkAsync(string customerId, [NotNull] Uri returnUrl, CancellationToken ct)
    {
        Stripe.BillingPortal.SessionCreateOptions options = new()
        {
            Customer = customerId,
            ReturnUrl = returnUrl.ToString(),
        };

        var billingSession = await billingSessionService.CreateAsync(options, cancellationToken: ct).ConfigureAwait(false);
        return billingSession is null ? null : new Uri(billingSession.Url);
    }
}
