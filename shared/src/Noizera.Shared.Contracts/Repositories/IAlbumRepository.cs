using Noizera.Shared.Domain.MusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface IAlbumRepository : IRepository<Album>
{
    Task<Album?> GetAsync(Guid albumId, CancellationToken ct);
    Task<Album?> GetLatestDraftAsync(Guid userId, CancellationToken ct);
    Task<Album?> GetFullAsync(Guid albumId, CancellationToken ct);
}
