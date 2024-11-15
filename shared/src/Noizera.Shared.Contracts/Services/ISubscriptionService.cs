using Microsoft.Extensions.Configuration;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.UserSubscriptions;

namespace Noizera.Common.Contracts.Services;

public interface ISubscriptionService
{
    Task<Uri> CreateCheckoutSessionAsync(User user, Subscription subscription, IConfiguration configuration, CancellationToken ct);

    Task InitializeCustomerAsync(User user, CancellationToken ct);

    Task StartSubscriptionAsync(string checkoutSessionId, UserSubscription userSubscription, CancellationToken ct);

    Task<Uri?> GetSubscriptionPortalUrlAsync(User user, IConfiguration configuration, CancellationToken ct);
}
