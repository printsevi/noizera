using Noizera.Common.Domain.MusicSets;

namespace Noizera.Common.Contracts.Repositories;

public interface IPlaylistRepository : IRepository<Playlist>
{
    Task<Playlist?> GetAsync(Guid playlistId, CancellationToken ct);

    Task<Playlist?> GetFavouritesAsync(Guid ownerId, CancellationToken ct);
}
