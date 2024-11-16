using Noizera.Common.Domain.UserSubscriptions;

namespace Noizera.Common.Contracts.Repositories;

public interface IUserSubscriptionRepository : IRepository<UserSubscription>
{
    Task<List<UserSubscription>> GetAllAsync(Guid userId, CancellationToken ct);
    Task<UserSubscription?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken ct);
}
