using Microsoft.Extensions.Configuration;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Domain.UserSubscriptions;
using Noizera.Shared.Infrastructure.Subscriptions;

namespace Noizera.Infrastructure.Subscriptions;

public class SubscriptionService(
    SubscriptionStripeService subscriptionStripeService)
    : ISubscriptionService
{
    public async Task<Uri> CreateCheckoutSessionAsync(User user, Subscription subscription, IConfiguration configuration, CancellationToken ct)
    {
        return await subscriptionStripeService.CreateCheckoutSessionAsync(user, subscription, configuration, ct);
    }

    public async Task InitializeCustomerAsync(User user, CancellationToken ct)
    {
        await subscriptionStripeService.CreateStripeCustomerAsync(user, ct);
    }

    public async Task StartSubscriptionAsync(string checkoutSessionId, UserSubscription userSubscription, CancellationToken ct)
    {
        await subscriptionStripeService.ActivateSubscriptionAsync(checkoutSessionId, userSubscription, ct);
    }

    public async Task<Uri?> GetSubscriptionPortalUrlAsync(User user, IConfiguration configuration, CancellationToken ct)
    {
        return await subscriptionStripeService.GetSubscriptionPortalUrlAsync(user, configuration, ct);
    }
}
