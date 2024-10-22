using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Songs;

public sealed class SongRepository(AppDbContext db, IHashGenerator hashGenerator) 
    : BaseEntityExtendedRepository<Song>(db, hashGenerator), ISongRepository
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
