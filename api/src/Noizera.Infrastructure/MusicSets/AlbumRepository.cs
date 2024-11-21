using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using Noizera.Infrastructure.Common;

namespace Noizera.Infrastructure.MusicSets;

public sealed class AlbumRepository(AppDbContext db)
    : BaseEntityExtendedRepository<Album>(db), IAlbumRepository
{
    public async Task<Album?> GetAsync(Guid albumId, CancellationToken ct)
        => await Db.Albums.FirstOrDefaultAsync(x => x.Id == albumId, ct).ConfigureAwait(false);

    public async Task<Album?> GetFullAsync(Guid albumId, CancellationToken ct) => await Db.Albums
            .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            .Include(x => x.Owner)
                .ThenInclude(x => x.Profile)
            //.Include(x => x.Credits)
            //    .ThenInclude(i => i.Profile)
            .FirstOrDefaultAsync(x => x.Id == albumId, ct).ConfigureAwait(false);

    public async Task<Album?> GetLatestDraftAsync(Guid userId, CancellationToken ct) => await Db.Albums
            .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                .ThenInclude(i => i.Song)
            //.Include(x => x.Credits)
            //    .ThenInclude(i => i.Profile)
            .FirstOrDefaultAsync(x => x.OwnerId == userId && x.AlbumStatus == AlbumStatus.Draft, ct).ConfigureAwait(false);
}
