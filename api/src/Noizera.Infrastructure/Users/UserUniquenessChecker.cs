using Microsoft.EntityFrameworkCore;
using Noizera.Common.Domain.Users;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Users;

public class UserUniquenessChecker(AppDbContext db) : IUserUniquenessChecker
{
    public async Task<bool> VerifyEmailAsync(string email, CancellationToken ct)
        => !await db.Users
        .AnyAsync(x => EF.Functions.ILike(x.Email, email), ct)
        .ConfigureAwait(false);
}
