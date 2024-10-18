using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Profiles;

public class ProfileUniquenessChecker(AppDbContext db) : IProfileUniquenessChecker
{
    public async Task<bool> VerifyUsernameAsync(string username, CancellationToken ct)
        => !await db.Profiles
        .AnyAsync(x => EF.Functions.ILike(x.PublicId, username), ct)
        .ConfigureAwait(false);
}
