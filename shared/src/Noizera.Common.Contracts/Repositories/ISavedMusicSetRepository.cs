using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Domain.SavedMusicSets;

namespace Noizera.Common.Contracts.Repositories;

public interface ISavedMusicSetRepository : IRepository<SavedMusicSet>
{
    Task<List<MusicSetCardQueryResult>> GetAllAsync(Guid userId, CancellationToken ct);

    Task DeleteAsync(Guid MusicSetId, Guid userId, CancellationToken ct);
}
