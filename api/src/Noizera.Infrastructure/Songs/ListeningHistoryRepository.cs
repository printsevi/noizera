using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.ListeningHistories;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Songs;

public sealed class ListeningHistoryRepository(AppDbContext db)
    : BaseEntityRepository<ListeningHistory>(db), IListeningHistoryRepository
{
    public async Task<ListeningHistory?> GetAsync(Guid userId, Guid songId) => await Db.ListeningHistories
            .FirstOrDefaultAsync(x => x.ListenerUserId == userId && x.SongId == songId).ConfigureAwait(false);
}
