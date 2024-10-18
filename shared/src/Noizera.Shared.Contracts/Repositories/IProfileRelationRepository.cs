using Noizera.Shared.Domain.ProfileRelations;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Contracts.Repositories;

public interface IProfileRelationRepository : IRepository<ProfileRelation>
{
    Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken ct);
}
