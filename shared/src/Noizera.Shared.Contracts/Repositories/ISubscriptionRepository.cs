using Noizera.Common.Domain.Subscriptions;

namespace Noizera.Common.Contracts.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetAsync(Guid subscriptionId, CancellationToken ct);

    Task<List<Subscription>> GetAllAsync(string? profileType, CancellationToken ct);
}
