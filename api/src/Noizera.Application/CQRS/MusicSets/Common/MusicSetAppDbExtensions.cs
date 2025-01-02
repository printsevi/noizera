using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.MusicSets.Common;

internal static class MusicSetAppDbExtensions
{
    public static async Task<List<MusicSetSongResult>> GetSongsAsync(this AppDbContext db, string musicSetPublicId, AudioType audioType, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                s."PublicId" as SongPublicId,
                s."Title" as Title,
                mcs."Sequence" as Sequence,
                CASE 
                    WHEN {audioType}::INTEGER = 1
                        THEN s."MpegContentLength"
                    ELSE s."FlacContentLength" 
                END AS ContentLength,
                s."DurationInSeconds" as DurationInSeconds,
                a."PublicId" as AlbumPublicId,
                p."Username" as OwnerUsername,
                p."Name" as OwnerName,
                CASE 
                    WHEN {userId}::UUID IS NOT NULL AND fav_mcs."Id" IS NOT NULL
                        THEN fav_mcs."Id"
                    ELSE NULL
                END AS FavouriteSongId
            FROM 
                public."Songs" s
            JOIN 
                public."MusicSetSongs" mcs
                    ON s."Id" = mcs."SongId"
            JOIN 
                public."MusicSets" a
                    ON a."Id" = s."AlbumId"
            JOIN 
                public."Profiles" p
                    ON p."UserId" = s."OwnerId"
            LEFT JOIN (
                SELECT 
                    fav_mcs_sub."Id",
                    fav_mcs_sub."SongId"
                FROM 
                    public."MusicSetSongs" fav_mcs_sub
                WHERE 
                    fav_mcs_sub."MusicSetId" = (
                        SELECT fav_mc."Id"
                        FROM public."MusicSets" fav_mc
                        WHERE 
                            fav_mc."CollectionType" = 'collection_playlist'
                            AND fav_mc."OwnerId" = {userId}::UUID
                            AND fav_mc."IsDeleted" = false
                            AND fav_mc."PlaylistTag" = 'favourites'
                        LIMIT 1
                    )
            ) fav_mcs
                ON fav_mcs."SongId" = s."Id"
            WHERE 
                mcs."MusicSetId" = (                                 
                    SELECT mc."Id"
                    FROM public."MusicSets" mc
                    WHERE 
                        mc."PublicId" = UPPER({musicSetPublicId})
                        AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                            OR mc."CollectionType" = 'collection_playlist')
                        AND mc."IsDeleted" = false
                    LIMIT 1
                )
            ORDER BY mcs."Sequence" ASC;
            """;

        var result = await db.Database
            .SqlQuery<MusicSetSongResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public static async Task<List<AlbumCreditQueryResult>> GetAlbumsCreditsAsync(this AppDbContext db, IEnumerable<string> musicSetPublicIds, CancellationToken ct)
    {
        var upperIds = musicSetPublicIds.Select(x => x.ToUpperInvariant());

        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as MusicSetPublicId,
                p."ProfileType" as ProfileType,
                p."Username" as Username,
                p."Name" as Name,
                ac."ProfileName" as ProfileName
            FROM 
                public."MusicSets" mc
            JOIN 
                public."AlbumCredits" ac
                    ON ac."AlbumId" = mc."Id"
            LEFT JOIN 
                public."Profiles" p
                    ON p."Id" = ac."ProfileId"
            WHERE 
                mc."PublicId" = ANY({upperIds})
                    AND mc."CollectionType" = 'collection_album' 
                    AND mc."AlbumStatus" = 'Released'
                    AND mc."IsDeleted" = FALSE
            """;

        var result = await db.Database
            .SqlQuery<AlbumCreditQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }
}

internal enum AudioType
{
    Mpeg = 1,
    Flac = 2
}
