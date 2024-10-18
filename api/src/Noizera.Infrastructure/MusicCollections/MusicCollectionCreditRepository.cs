using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.MusicCollectionCredits;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.MusicCollections;

public sealed class MusicCollectionCreditRepository(AppDbContext db) : BaseRepository<MusicCollectionCredit>(db), IMusicCollectionCreditRepository
{
    public async Task DeleteCreditAsync(Guid creditId, CancellationToken ct) => await Db.MusicCollectionCredits
            .Where(x => x.Id == creditId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);
}
