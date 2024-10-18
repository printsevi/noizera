using Microsoft.Extensions.Configuration;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Domain.UserSubscriptions;

namespace Noizera.Shared.Contracts.Services;

public interface ISubscriptionService
{
    Task<Uri> CreateCheckoutSessionAsync(User user, Subscription subscription, IConfiguration configuration, CancellationToken ct);
    
    Task InitializeCustomerAsync(User user, CancellationToken ct);

    Task StartSubscriptionAsync(string checkoutSessionId, UserSubscription userSubscription, CancellationToken ct);

    Task<Uri?> GetSubscriptionPortalUrlAsync(User user, IConfiguration configuration, CancellationToken ct);
}
