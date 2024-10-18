using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Subscriptions;

public sealed class SubscriptionRepository(AppDbContext db) 
    : BaseRepository<Subscription>(db), ISubscriptionRepository
{
    public async Task<Subscription?> GetAsync(Guid subscriptionId, CancellationToken ct) => await Db.Subscriptions
            .FirstOrDefaultAsync(x => x.Id == subscriptionId, ct).ConfigureAwait(false);

    public async Task<List<Subscription>> GetAllAsync(string? profileType, CancellationToken ct) => await Db.Subscriptions
            .Where(x => !x.IsDisabled && (string.IsNullOrWhiteSpace(profileType) || x.ProfileTypes.Contains(profileType)))
            .ToListAsync(ct).ConfigureAwait(false);
}
