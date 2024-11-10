using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Domain.SavedMusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface ISavedMusicSetRepository : IRepository<SavedMusicSet>
{
    Task<List<MusicSetCardQueryResult>> GetAllAsync(Guid userId, CancellationToken ct);

    Task DeleteAsync(Guid MusicSetId, Guid userId, CancellationToken ct);
}
