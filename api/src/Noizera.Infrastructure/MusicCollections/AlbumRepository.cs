using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.MusicCollections;

public sealed class AlbumRepository(AppDbContext db) : BaseRepository<Album>(db), IAlbumRepository
{
    public async Task<Album?> GetAsync(Guid albumId, CancellationToken ct)
    {
        return await Db.Albums.FirstOrDefaultAsync(x => x.Id == albumId, ct).ConfigureAwait(false);
    }

    public async Task<Album?> GetFullAsync(Guid albumId, CancellationToken ct) => await Db.Albums
            .Include(x => x.MusicCollectionSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            .Include(x => x.Owner)
                .ThenInclude(x => x.Profile)
            //.Include(x => x.Credits)
            //    .ThenInclude(i => i.Profile)
            .FirstOrDefaultAsync(x => x.Id == albumId, ct).ConfigureAwait(false);

    public async Task<Album?> GetLatestDraftAsync(Guid userId, CancellationToken ct) => await Db.Albums
            .Include(x => x.MusicCollectionSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            .Include(x => x.Credits)
                .ThenInclude(i => i.Profile)
            .FirstOrDefaultAsync(x => x.OwnerId == userId && x.AlbumStatus == AlbumStatus.Draft, ct).ConfigureAwait(false);
}
