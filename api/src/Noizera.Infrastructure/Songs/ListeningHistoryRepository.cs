using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Songs;

public sealed class ListeningHistoryRepository(AppDbContext db)
    : BaseRepository<ListeningHistory>(db), IListeningHistoryRepository
{
    public async Task<ListeningHistory?> GetAsync(Guid userId, Guid songId) => await Db.ListeningHistories
            .FirstOrDefaultAsync(x => x.ListenerUserId == userId && x.SongId == songId).ConfigureAwait(false);
}
