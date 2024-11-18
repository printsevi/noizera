using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.SecretTokens;
using Noizera.Common.Domain.Users;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Users;

public sealed class UserRepository(AppDbContext db) : BaseEntityRepository<User>(db), IUserRepository
{
    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken ct) => await Db.Users
        .Include(u => u.Profile)
        .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, emailOrUsername) || EF.Functions.ILike(u.Profile!.PublicId, emailOrUsername), ct).ConfigureAwait(false);

    public async Task<User?> GetAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Include(u => u.Profile)
        .FirstOrDefaultAsync(u => u.Id == userId, ct).ConfigureAwait(false);

    public async Task<User?> GetWithSavedCollectionsAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Include(u => u.SavedMusicSets)
        .FirstOrDefaultAsync(u => u.Id == userId, ct).ConfigureAwait(false);

    public async Task<User?> GetWithSongsAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Include(u => u.Profile)
        .Include(u => u.Songs)
        .FirstOrDefaultAsync(u => u.Id == userId, ct).ConfigureAwait(false);

    public async Task<User?> GetWithActiveRefreshTokensAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Include(u => u.SecretTokens.Where(x => x.TokenType == SecretTokenType.Refresh && !x.IsRevoked && SystemClock.UtcNow < x.ExpireAt))
        .FirstOrDefaultAsync(u => u.Id == userId, ct).ConfigureAwait(false);

    public async Task<User?> GetByEmailWithLatestResetTokenAsync(string email, CancellationToken ct) => await Db.Users
        .Include(u => u.SecretTokens.Where(x => x.TokenType == SecretTokenType.Reset && !x.IsRevoked))
        .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email), ct).ConfigureAwait(false);

    public async Task<User?> GetWithSubscriptionsAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Include(u => u.Profile)
        .Include(u => u.Subscriptions)
            .ThenInclude(u => u.Subscription)
        .FirstOrDefaultAsync(u => u.Id == userId, ct).ConfigureAwait(false);

    public async Task<MyUserQueryResult?> GetMyUserAsync(Guid userId, CancellationToken ct) => await Db.Users
        .Where(u => u.Id == userId)
        .Select(u => new MyUserQueryResult(u.Profile!.ProfileType.ToString(), u.Profile!.PublicId, u.Profile!.Name, u.ActiveSubscriptionTypes, u.Songs.Count, true))
        .FirstOrDefaultAsync(ct)
        .ConfigureAwait(false);
}
