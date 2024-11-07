using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Domain.MusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface IMusicCollectionRepository : IRepository<MusicSet>
{
    Task<MusicSet?> GetAsync(Guid id, CancellationToken ct);

    Task<MusicSet?> GetAsync(string publicId, CancellationToken ct);

    Task<MusicSet?> GetWithSongsAsync(Guid musicCollectionId, CancellationToken ct);

    Task<List<MusicCollectionSongResult>> GetFlacSongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct);

    Task<List<MusicCollectionSongResult>> GetMp3SongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct);

    Task<MusicCollectionQueryResult?> GetMusicCollectionAsync(string collectionPublicId, Guid? userId, CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetRecommendationsAsync(CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetNewReleasesAsync(Guid userId, CancellationToken ct);

    Task<List<MusicCollectionCardQueryResult>> GetNewReleasesAsync(CancellationToken ct);
}
