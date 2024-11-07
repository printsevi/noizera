using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.MusicCollections;

public sealed class MusicCollectionRepository(AppDbContext db, IHashGenerator hashGenerator) 
    : BaseEntityExtendedRepository<MusicSet>(db, hashGenerator), IMusicCollectionRepository
{
    public async Task<MusicSet?> GetAsync(Guid id, CancellationToken ct)
    {
        return await Db.MusicCollections.FirstOrDefaultAsync(x => x.Id == id, ct).ConfigureAwait(false);
    }

    public async Task<MusicSet?> GetAsync(string publicId, CancellationToken ct)
    {
        return await Db.MusicCollections.FirstOrDefaultAsync(x => x.PublicId == publicId, ct).ConfigureAwait(false);
    }

    public async Task<MusicSet?> GetWithSongsAsync(Guid musicCollectionId, CancellationToken ct) => await Db.MusicCollections
            .Include(x => x.MusicCollectionSongs)
            .FirstOrDefaultAsync(x => x.Id == musicCollectionId, ct).ConfigureAwait(false);

    public async Task<MusicCollectionQueryResult?> GetMusicCollectionAsync(string collectionPublicId, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                mc."Description" as Description,
                mc."AlbumReleaseDate" as ReleaseDate
            FROM 
                public."MusicCollections" mc
            WHERE 
                mc."PublicId" = {collectionPublicId}
                    AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                        OR mc."CollectionType" = 'collection_playlist')
                    AND mc."IsDeleted" = false
            LIMIT 1
         """;

        var result = await Db.Database
            .SqlQuery<MusicCollectionQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    public async Task<MusicCollectionQueryResult?> GetMusicCollection2Async(string collectionPublicId, Guid? userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                s."PublicId" as SongPublicId,
                s."Title" as Title,
                mcs."Sequence" as Sequence
                CASE 
                    WHEN {userId} IS NULL 
                        THEN NULL
                    WHEN fav_mcs."SongId" IS NOT NULL 
                        THEN true
                    ELSE false 
                END AS IsFavourite
            FROM 
                public."Songs" s
            JOIN 
                public."MusicCollectionSongs" mcs
                    ON s."Id" = mcs."SongId"
            JOIN 
                public."MusicCollections" mc
                    ON mc."Id" = mcs."MusicCollectionId"
            LEFT JOIN 
                public."MusicCollectionSongs" fav_mcs
                    ON fav_mcs."SongId" = s."Id"
                    AND ({userId} IS NULL 
                        OR fav_mcs."MusicCollectionId" = (
                            SELECT 
                                fav_mc."Id"
                            FROM 
                                public."MusicCollections" fav_mc
                            WHERE 
                                fav_mc."CollectionType" = 'collection_playlist'
                                    AND fav_mc."OwnerId" = {userId}
                                    AND fav_mc."IsDeleted" = false
                                    AND fav_mc."PlaylistTag" = 'favourites'
                            LIMIT 1
                        )
                    )
            WHERE 
                mcs."MusicCollectionId" = (                                 
                    SELECT mc."Id"
                    FROM public."MusicCollections" mc
                    WHERE 
                        mc."PublicId" = {collectionPublicId}
                            AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                                OR mc."CollectionType" = 'collection_playlist')
                            AND mc."IsDeleted" = false
                    LIMIT 1
                );
            """;

        var result = await Db.Database
            .SqlQuery<MusicCollectionQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    public async Task<List<MusicCollectionSongResult>> GetFlacSongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                s."PublicId" as SongPublicId,
                s."Title" as Title,
                mcs."Sequence" as Sequence,
                s."FlacContentLength" as ContentLength,
                s."DurationInSeconds" as DurationInSeconds
            FROM 
                public."Songs" s
            JOIN 
                public."MusicCollectionSongs" mcs
                    ON s."Id" = mcs."SongId"
            WHERE 
                mcs."MusicCollectionId" = (                                 
                    SELECT mc."Id"
                    FROM public."MusicCollections" mc
                    WHERE 
                        mc."PublicId" = {collectionPublicId}
                        AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                            OR mc."CollectionType" = 'collection_playlist')
                        AND mc."IsDeleted" = false
                    LIMIT 1
                )
            ORDER BY mcs."Sequence" ASC;
            """;

        var result = await Db.Database
            .SqlQuery<MusicCollectionSongResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<List<MusicCollectionSongResult>> GetMp3SongsByCollectionPublicIdAsync(string collectionPublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                s."PublicId" as SongPublicId,
                s."Title" as Title,
                mcs."Sequence" as Sequence,
                s."Mp3ContentLength" as ContentLength,
                s."DurationInSeconds" as DurationInSeconds
            FROM 
                public."Songs" s
            JOIN 
                public."MusicCollectionSongs" mcs
                    ON s."Id" = mcs."SongId"
            WHERE 
                mcs."MusicCollectionId" = (                                 
                    SELECT mc."Id"
                    FROM public."MusicCollections" mc
                    WHERE 
                        mc."PublicId" = {collectionPublicId}
                        AND ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                            OR mc."CollectionType" = 'collection_playlist')
                        AND mc."IsDeleted" = false
                    LIMIT 1
                )
            ORDER BY mcs."Sequence" ASC;
            """;

        var result = await Db.Database
            .SqlQuery<MusicCollectionSongResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<List<MusicCollectionCardQueryResult>> GetRecommendationsAsync(Guid userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."PublicId" as OwnerPublicId,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                CASE 
                    WHEN smc."UserId" IS NOT NULL 
                    THEN true
                    ELSE false 
                END AS IsSaved
            FROM 
                public."MusicCollections" mc
            LEFT JOIN 
                public."SavedMusicCollections" smc
                    ON smc."MusicSetId" = mc."Id"
                    AND smc."UserId" = {userId}
            LEFT JOIN 
                public."Users" u
                    ON mc."OwnerId" = u."Id"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicCollectionSongs" mcs
                    ON mcs."MusicCollectionId" = mc."Id"
            WHERE 
                (mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                AND mc."IsDeleted" = false
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."PublicId", mc."Id", smc."UserId"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicCollectionCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicCollectionCardQueryResult>> GetRecommendationsAsync(CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."PublicId" as OwnerPublicId,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                FALSE as IsSaved
            FROM 
                public."MusicCollections" mc
            LEFT JOIN 
                public."Users" u
                    ON mc."OwnerId" = u."Id"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicCollectionSongs" mcs
                    ON mcs."MusicCollectionId" = mc."Id"
            WHERE 
                (mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                AND mc."IsDeleted" = false
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."PublicId", mc."Id"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicCollectionCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicCollectionCardQueryResult>> GetNewReleasesAsync(Guid userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."PublicId" as OwnerPublicId,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                CASE 
                    WHEN smc."UserId" IS NOT NULL 
                    THEN true
                    ELSE false 
                END AS IsSaved
            FROM 
                (SELECT * 
                 FROM public."MusicCollections" mc1
                 WHERE 
                    (mc1."CollectionType" = 'collection_album' AND mc1."AlbumStatus" = 'Released')
                    AND mc1."IsDeleted" = false
                 ORDER BY mc1."CreatedAt" DESC
                 LIMIT 50) mc
            LEFT JOIN 
                public."SavedMusicCollections" smc
                    ON smc."MusicSetId" = mc."Id"
                    AND smc."UserId" = {userId}
            LEFT JOIN 
                public."Users" u
                    ON mc."OwnerId" = u."Id"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicCollectionSongs" mcs
                    ON mcs."MusicCollectionId" = mc."Id"
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."PublicId", mc."Id", smc."UserId"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicCollectionCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<List<MusicCollectionCardQueryResult>> GetNewReleasesAsync(CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."PublicId" as OwnerPublicId,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                FALSE as IsSaved
            FROM 
                (SELECT * 
                 FROM public."MusicCollections" mc1
                 WHERE 
                    (mc1."CollectionType" = 'collection_album' AND mc1."AlbumStatus" = 'Released')
                    AND mc1."IsDeleted" = false
                 ORDER BY mc1."CreatedAt" DESC
                 LIMIT 50) mc
            LEFT JOIN 
                public."Users" u
                    ON mc."OwnerId" = u."Id"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicCollectionSongs" mcs
                    ON mcs."MusicCollectionId" = mc."Id"
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."PublicId", mc."Id"
            ORDER BY RANDOM()
            LIMIT 20
            """;

        return await Db.Database
            .SqlQuery<MusicCollectionCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }
}
