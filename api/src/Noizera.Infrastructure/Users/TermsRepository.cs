using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Terms;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Users;

public sealed class TermsRepository(AppDbContext db) : BaseRepository<TermsOfUse>(db), ITermsRepository
{
    public async Task<TermsOfUse?> GetLatestAsync(CancellationToken ct)
    {
        return await Db.Terms
                       .OrderByDescending(p => p.EffectiveDate)
                       .FirstOrDefaultAsync(ct).ConfigureAwait(false);
    }
}
