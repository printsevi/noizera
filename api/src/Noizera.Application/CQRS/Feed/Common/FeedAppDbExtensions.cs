using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed.Common;

internal static class FeedAppDbExtensions
{
    public static async Task<List<ProfileCardQueryResult>> GetProfilesAsync(this AppDbContext db, ProfileType profileType, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                p."Username" as Username,
                p."PublicId" as PublicId,
                p."ProfileType" as ProfileType,
                p."Name" as Name
            FROM 
                public."Profiles" p
            WHERE 
                (p."ProfileType" = 'Artist' AND {profileType}::INTEGER = 1)
                OR (p."ProfileType" = 'Label' AND {profileType}::INTEGER = 2)
            ORDER BY RANDOM()
            LIMIT 20;
        """;

        var result = await db.Database
            .SqlQuery<ProfileCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }
}

internal enum ProfileType
{
    Artist = 1,
    Label = 2
}
