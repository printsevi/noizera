using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.MusicSets;

public sealed class PlaylistRepository(AppDbContext db, IHashGenerator hashGenerator)
    : BaseEntityExtendedRepository<Playlist>(db, hashGenerator), IPlaylistRepository
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
