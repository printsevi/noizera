using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Profiles.Common;

internal static class ProfilesAppDbExtensions
{
    public static async Task<List<MusicSetQueryResult>> GetProfileMusicSetsAsync(this AppDbContext db, string username, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
                SELECT 
                    mc."PublicId" as PublicId,
                    mc."Title" as Title,
                    mc."CollectionType" as CollectionType,
                    p."Username" as OwnerUsername,
                    p."Name" as OwnerName,
                    COUNT(mcs."Id") AS SongCount,
                    p."ProfileType" as OwnerProfileType,
                    mc."AlbumReleaseDate" as ReleaseDate,
                    CASE 
                        WHEN smc."UserId" IS NOT NULL 
                        THEN true
                        ELSE false 
                    END AS IsSaved
                FROM 
                    public."MusicSets" mc
                LEFT JOIN 
                    public."SavedMusicSets" smc
                        ON smc."MusicSetId" = mc."Id"
                        AND smc."UserId" = {userId}
                LEFT JOIN 
                    public."Profiles" p
                        ON p."UserId" = mc."OwnerId"
                LEFT JOIN 
                    public."MusicSetSongs" mcs
                        ON mcs."MusicSetId" = mc."Id"
                WHERE 
                    (mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                    AND p."Username" = UPPER({username})
                    AND mc."IsDeleted" = false
                GROUP BY 
                    mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", smc."UserId", p."ProfileType", mc."AlbumReleaseDate"
                ORDER BY mc."Id" DESC
                LIMIT 100
            """;

        return await db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }
}
