using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Terms;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Users;

public sealed class TermsRepository(AppDbContext db) : BaseEntityRepository<TermsOfUse>(db), ITermsRepository
{
    public async Task<TermsOfUse?> GetLatestAsync(CancellationToken ct)
    {
        return await Db.Terms
                       .OrderByDescending(p => p.EffectiveDate)
                       .FirstOrDefaultAsync(ct).ConfigureAwait(false);
    }
}
