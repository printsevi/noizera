using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Domain.MusicSets;

namespace Noizera.Common.Contracts.Repositories;

public interface IMusicSetRepository : IRepository<MusicSet>
{
    Task<MusicSet?> GetAsync(Guid id, CancellationToken ct);

    Task<MusicSet?> GetAsync(string publicId, CancellationToken ct);

    Task<MusicSet?> GetWithSongsAsync(Guid MusicSetId, CancellationToken ct);

    Task<List<MusicSetSongResult>> GetFlacSongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct);

    Task<List<MusicSetSongResult>> GetMp3SongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct);

    Task<MusicSetQueryResult?> GetMusicSetAsync(string collectionPublicId, Guid? userId, CancellationToken ct);

    Task<List<MusicSetCardQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct);

    Task<List<MusicSetCardQueryResult>> GetRecommendationsAsync(CancellationToken ct);

    Task<List<MusicSetCardQueryResult>> GetNewReleasesAsync(Guid userId, CancellationToken ct);

    Task<List<MusicSetCardQueryResult>> GetNewReleasesAsync(CancellationToken ct);
}
