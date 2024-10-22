using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.UserSubscriptions;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Subscriptions;

public sealed class UserSubscriptionRepository(AppDbContext db) : BaseEntityRepository<UserSubscription>(db), IUserSubscriptionRepository
{
    public async Task<List<UserSubscription>> GetAllAsync(Guid userId, CancellationToken ct) => await Db.UserSubscriptions
            .Include(x => x.Subscription)
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync(ct).ConfigureAwait(false);

    public async Task<UserSubscription?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken ct) => await Db.UserSubscriptions
            .Include(x => x.Subscription)
            .FirstOrDefaultAsync(x => x.CheckoutSessionId == checkoutSessionId, ct).ConfigureAwait(false);
}
