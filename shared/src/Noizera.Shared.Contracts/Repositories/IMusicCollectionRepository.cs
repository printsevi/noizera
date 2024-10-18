using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Domain.MusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface IMusicCollectionRepository : IRepository<MusicSet>
{
    Task<MusicSet?> GetAsync(Guid id, CancellationToken ct);

    Task<MusicSet?> GetWithSongsAsync(Guid musicCollectionId, CancellationToken ct);

    Task<List<MusicCollectionSongResult>> GetSongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct);

    Task<MusicCollectionQueryResult?> GetMusicCollectionAsync(string collectionPublicId, Guid? userId, CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetPublicRecommendationsAsync(CancellationToken ct);
}
