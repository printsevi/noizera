using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Domain.SavedMusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface ISavedMusicCollectionRepository : IRepository<SavedMusicSet>
{
    Task<List<MusicCollectionCardQueryResult>> GetAllAsync(Guid userId, CancellationToken ct);
}
