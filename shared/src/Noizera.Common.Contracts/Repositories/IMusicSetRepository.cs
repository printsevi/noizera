using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Domain.MusicSets;

namespace Noizera.Common.Contracts.Repositories;

public interface IMusicSetRepository : IRepository<MusicSet>
{
    Task<MusicSet?> GetAsync(Guid id, CancellationToken ct);

    Task<MusicSet?> GetAsync(string publicId, CancellationToken ct);

    Task<MusicSet?> GetWithSongsAsync(Guid MusicSetId, CancellationToken ct);

    Task<MusicSetQueryResult?> GetMusicSetAsync(string collectionPublicId, Guid? userId, CancellationToken ct);

    Task<List<MusicSetQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct);

    Task<List<MusicSetQueryResult>> GetRecommendationsAsync(CancellationToken ct);

    Task<List<MusicSetQueryResult>> GetNewReleasesAsync(Guid userId, CancellationToken ct);

    Task<List<MusicSetQueryResult>> GetNewReleasesAsync(CancellationToken ct);
}
