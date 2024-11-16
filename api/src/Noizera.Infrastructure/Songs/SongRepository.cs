using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Persistence.SQL;
using Noizera.Infrastructure.Common;

namespace Noizera.Infrastructure.Songs;

public sealed class SongRepository(AppDbContext db)
    : BaseEntityExtendedRepository<Song>(db), ISongRepository
{
    public async Task<Song?> GetAsync(Guid id, CancellationToken ct) => await Db.Songs
            .FirstOrDefaultAsync(x => x.Id == id, ct).ConfigureAwait(false);

    public async Task<Song?> GetAsync(string publicId, CancellationToken ct) => await Db.Songs
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct).ConfigureAwait(false);

    public async Task DeleteAsync(Guid songId, CancellationToken ct)
    {
        _ = await Db.Songs
            .Where(x => x.Id == songId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);

        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
