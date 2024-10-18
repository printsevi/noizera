using Noizera.Shared.Domain.MusicSets;

namespace Noizera.Shared.Contracts.Repositories;

public interface IPlaylistRepository : IRepository<Playlist>
{
    Task<Playlist?> GetAsync(Guid playlistId, CancellationToken ct);

    Task<Playlist?> GetFavouritesAsync(Guid ownerId, CancellationToken ct);
}
