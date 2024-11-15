using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Subscriptions;

public sealed class SubscriptionRepository(AppDbContext db)
    : BaseEntityRepository<Subscription>(db), ISubscriptionRepository
{
    public async Task<Subscription?> GetAsync(Guid subscriptionId, CancellationToken ct) => await Db.Subscriptions
            .FirstOrDefaultAsync(x => x.Id == subscriptionId, ct).ConfigureAwait(false);

    public async Task<List<Subscription>> GetAllAsync(string? profileType, CancellationToken ct) => await Db.Subscriptions
            .Where(x => !x.IsDisabled && (string.IsNullOrWhiteSpace(profileType) || x.ProfileTypes.Contains(profileType)))
            .ToListAsync(ct).ConfigureAwait(false);
}
