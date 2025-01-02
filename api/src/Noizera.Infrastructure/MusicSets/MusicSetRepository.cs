using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using Noizera.Infrastructure.Common;

namespace Noizera.Infrastructure.MusicSets;

public sealed class MusicSetRepository(AppDbContext db)
    : BaseEntityExtendedRepository<MusicSet>(db), IMusicSetRepository
{
    public async Task<MusicSet?> GetAsync(Guid id, CancellationToken ct)
        => await Db.MusicSets.FirstOrDefaultAsync(x => x.Id == id, ct).ConfigureAwait(false);

    public async Task<MusicSet?> GetAsync(string publicId, CancellationToken ct)
        => await Db.MusicSets.FirstOrDefaultByPublicIdAsync(publicId, ct).ConfigureAwait(false);

    public async Task<MusicSet?> GetWithSongsAsync(Guid MusicSetId, CancellationToken ct) => await Db.MusicSets
            .Include(x => x.MusicSetSongs)
            .FirstOrDefaultAsync(x => x.Id == MusicSetId, ct).ConfigureAwait(false);

    public async Task<MusicSetQueryResult?> GetMusicSetAsync(string collectionPublicId, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                mc."AlbumReleaseDate" as ReleaseDate,
                p."Username" as OwnerUsername,
                p."Name" as OwnerName,
                p."ProfileType" as OwnerProfileType,
                COUNT(mcs."Id") AS SongCount,
                CASE 
                    WHEN smc."UserId" IS NOT NULL 
                    THEN true
                    ELSE false 
                END AS IsSaved
            FROM 
                public."MusicSets" mc
            JOIN 
                public."Profiles" p
                    ON p."UserId" = mc."OwnerId"
             LEFT JOIN 
                 public."SavedMusicSets" smc
                     ON smc."MusicSetId" = mc."Id"
                     AND smc."UserId" = {userId}
            LEFT JOIN 
                public."MusicSetSongs" mcs
                    ON mcs."MusicSetId" = mc."Id"
            WHERE 
                mc."PublicId" = UPPER({collectionPublicId})
                AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                    OR (mc."CollectionType" = 'collection_playlist' AND (mc."IsPublicPlaylist" = TRUE OR mc."OwnerId" = {userId}))
                )
                AND mc."IsDeleted" = FALSE
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", smc."UserId", p."ProfileType", mc."AlbumReleaseDate"
            LIMIT 1
         """;

        var result = await Db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    public async Task<List<MusicSetQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                mc."AlbumReleaseDate" as ReleaseDate,
                p."Username" as OwnerUsername,
                p."Name" as OwnerName,
                p."ProfileType" as OwnerProfileType,
                COUNT(mcs."Id") AS SongCount,
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
                AND mc."IsDeleted" = false
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", smc."UserId", p."ProfileType", mc."AlbumReleaseDate"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicSetQueryResult>> GetRecommendationsAsync(CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                mc."AlbumReleaseDate" as ReleaseDate,
                p."Username" as OwnerUsername,
                p."Name" as OwnerName,
                p."ProfileType" as OwnerProfileType,
                COUNT(mcs."Id") AS SongCount,
                FALSE as IsSaved
            FROM 
                public."MusicSets" mc
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = mc."OwnerId"
            LEFT JOIN 
                public."MusicSetSongs" mcs
                    ON mcs."MusicSetId" = mc."Id"
            WHERE 
                (mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                AND mc."IsDeleted" = false
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", p."ProfileType", mc."AlbumReleaseDate"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicSetQueryResult>> GetNewReleasesAsync(Guid userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."Username" as OwnerUsername,
                p."ProfileType" as OwnerProfileType,
                mc."AlbumReleaseDate" as ReleaseDate,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                CASE 
                    WHEN smc."UserId" IS NOT NULL 
                    THEN true
                    ELSE false 
                END AS IsSaved
            FROM 
                (SELECT * 
                 FROM public."MusicSets" mc1
                 WHERE 
                    (mc1."CollectionType" = 'collection_album' AND mc1."AlbumStatus" = 'Released')
                    AND mc1."IsDeleted" = false
                 ORDER BY mc1."Id" DESC
                 LIMIT 50) mc
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
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", smc."UserId", p."ProfileType", mc."AlbumReleaseDate"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicSetQueryResult>> GetNewReleasesAsync(CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."Username" as OwnerUsername,
                p."Name" as OwnerName,
                p."ProfileType" as OwnerProfileType,
                mc."AlbumReleaseDate" as ReleaseDate,
                COUNT(mcs."Id") AS SongCount,
                FALSE as IsSaved
            FROM 
                (SELECT * 
                 FROM public."MusicSets" mc1
                 WHERE 
                    (mc1."CollectionType" = 'collection_album' AND mc1."AlbumStatus" = 'Released')
                    AND mc1."IsDeleted" = false
                 ORDER BY mc1."Id" DESC
                 LIMIT 50) mc
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = mc."OwnerId"
            LEFT JOIN 
                public."MusicSetSongs" mcs
                    ON mcs."MusicSetId" = mc."Id"
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username", mc."Id", p."ProfileType", mc."AlbumReleaseDate"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicSetQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }
}
