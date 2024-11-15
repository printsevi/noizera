using Microsoft.Extensions.Configuration;
using Noizera.Common.Contracts.Services;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.UserSubscriptions;
using Noizera.Common.Infrastructure.Subscriptions;

namespace Noizera.Infrastructure.Subscriptions;

public class SubscriptionService(
    SubscriptionStripeService subscriptionStripeService)
    : ISubscriptionService
{
    public async Task<Uri> CreateCheckoutSessionAsync(User user, Subscription subscription, IConfiguration configuration, CancellationToken ct)
        => await subscriptionStripeService.CreateCheckoutSessionAsync(user, subscription, configuration, ct).ConfigureAwait(false);

    public async Task InitializeCustomerAsync(User user, CancellationToken ct)
        => await subscriptionStripeService.CreateStripeCustomerAsync(user, ct).ConfigureAwait(false);

    public async Task StartSubscriptionAsync(string checkoutSessionId, UserSubscription userSubscription, CancellationToken ct)
        => await subscriptionStripeService.ActivateSubscriptionAsync(checkoutSessionId, userSubscription, ct).ConfigureAwait(false);

    public async Task<Uri?> GetSubscriptionPortalUrlAsync(User user, IConfiguration configuration, CancellationToken ct)
        => await subscriptionStripeService.GetSubscriptionPortalUrlAsync(user, configuration, ct).ConfigureAwait(false);
}
