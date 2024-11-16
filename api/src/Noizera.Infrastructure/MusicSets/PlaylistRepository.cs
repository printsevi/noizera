using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using Noizera.Infrastructure.Common;

namespace Noizera.Infrastructure.MusicSets;

public sealed class PlaylistRepository(AppDbContext db)
    : BaseEntityExtendedRepository<Playlist>(db), IPlaylistRepository
{
    public async Task<Playlist?> GetAsync(Guid playlistId, CancellationToken ct)
    {
        var result = await Db.Playlists
            .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            .FirstOrDefaultAsync(x => x.Id == playlistId, ct).ConfigureAwait(false);

        return result;
    }

    public async Task<Playlist?> GetFavouritesAsync(Guid ownerId, CancellationToken ct)
    {
        var result = await Db.Playlists
            .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            .FirstOrDefaultAsync(x => x.PlaylistTag == PlaylistConstants.FavouritesPlaylistTag && x.OwnerId == ownerId, ct).ConfigureAwait(false);

        return result;
    }
}
