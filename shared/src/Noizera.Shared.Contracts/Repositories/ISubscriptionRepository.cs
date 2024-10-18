using Noizera.Shared.Domain.Subscriptions;

namespace Noizera.Shared.Contracts.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetAsync(Guid subscriptionId, CancellationToken ct);

    Task<List<Subscription>> GetAllAsync(string? profileType, CancellationToken ct);
}
