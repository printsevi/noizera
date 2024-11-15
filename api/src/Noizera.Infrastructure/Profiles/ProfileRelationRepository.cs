using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.ProfileRelations;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Profiles;

public sealed class ProfileRelationRepository(AppDbContext db) : BaseEntityRepository<ProfileRelation>(db), IProfileRelationRepository
{
    public async Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken ct)
    {
        _ = await Db.ProfileRelations
            .Where(x => x.FollowerProfileId == followerId && x.FollowingProfileId == followingId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);

        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
