using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.ProfileRelations;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Profiles;

public sealed class ProfileRelationRepository(AppDbContext db) : BaseRepository<ProfileRelation>(db), IProfileRelationRepository
{
    public async Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken ct)
    {
        _ = await Db.ProfileRelations
            .Where(x => x.FollowerProfileId == followerId && x.FollowingProfileId == followingId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);

        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
