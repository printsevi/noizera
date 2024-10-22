using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.VerificationCodes;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Security.VerificationCodes;

public sealed class VerificationCodeRepository(AppDbContext db) : BaseEntityRepository<VerificationCode>(db), IVerificationCodeRepository
{
    public async Task<VerificationCode?> GetLatestAsync(string key, CancellationToken ct) => await Db.VerificationCodes
        .Where(x => EF.Functions.ILike(x.Key, key))
        .OrderByDescending(r => r.CreatedAt)
        .FirstOrDefaultAsync(ct);
}
