using Noizera.Common.Domain.MusicSets;

namespace Noizera.Common.Contracts.Repositories;

public interface IAlbumRepository : IRepository<Album>
{
    Task<Album?> GetAsync(Guid albumId, CancellationToken ct);
    Task<Album?> GetLatestDraftAsync(Guid userId, CancellationToken ct);
    Task<Album?> GetFullAsync(Guid albumId, CancellationToken ct);
}
