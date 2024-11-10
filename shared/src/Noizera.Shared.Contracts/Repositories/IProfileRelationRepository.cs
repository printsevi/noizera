using Noizera.Shared.Domain.ProfileRelations;

namespace Noizera.Shared.Contracts.Repositories;

public interface IProfileRelationRepository : IRepository<ProfileRelation>
{
    Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken ct);
}
