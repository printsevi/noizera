using Stripe;
using Stripe.Checkout;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Infrastructure.Subscriptions;

public class StripeService(
    SessionService sessionService,
    CustomerService customerService,
    Stripe.SubscriptionService subscriptionService,
    Stripe.AccountService accountService,
    Stripe.AccountLinkService accountLinkService,
    AccountLoginLinkService loginLinkService,
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

    public async Task<Stripe.Subscription?> GetSubscriptionAsync(string subscriptionId, CancellationToken ct)
    {
        var subscription = await subscriptionService.GetAsync(subscriptionId, cancellationToken: ct).ConfigureAwait(false);
        return subscription;
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

    public async Task<Account?> CreateConnectedAccountAsync(CancellationToken ct)
    {
        AccountCreateOptions accountOptions = new()
        {
            Type = "standard",
        };

        var account = await accountService.CreateAsync(accountOptions, cancellationToken: ct).ConfigureAwait(false);

        return account;
    }

    public async Task<Account?> GetConnectedAccountAsync(string accountId, CancellationToken ct)
    {
        var account = await accountService.GetAsync(accountId, cancellationToken: ct).ConfigureAwait(false);

        return account;
    }

    public async Task<Uri?> GetOnboardingConnectedAccountLinkAsync(string accountId, [NotNull] Uri refreshUrl, [NotNull] Uri returnUrl, CancellationToken ct)
    {
        AccountLinkCreateOptions accountLinkOptions = new()
        {
            Account = accountId,
            RefreshUrl = refreshUrl.ToString(),
            ReturnUrl = returnUrl.ToString(),
            Type = "account_onboarding"
        };

        var accountLink = await accountLinkService.CreateAsync(accountLinkOptions, cancellationToken: ct).ConfigureAwait(false);

        return accountLink?.Url is null ? null : new Uri(accountLink.Url);
    }

    public async Task<Uri?> GetLoginAccountLinkAsync(string accountId, CancellationToken ct)
    {
        AccountLoginLinkCreateOptions loginLinkOptions = new();
        var loginLink = await loginLinkService.CreateAsync(accountId, loginLinkOptions, cancellationToken: ct).ConfigureAwait(false);
        return loginLink?.Url is null ? null : new Uri(loginLink.Url);
    }
}
