using Noizera.Common.Domain.ProfileRelations;

namespace Noizera.Common.Contracts.Repositories;

public interface IProfileRelationRepository : IRepository<ProfileRelation>
{
    Task DeleteAsync(Guid followerId, Guid followingId, CancellationToken ct);
}
