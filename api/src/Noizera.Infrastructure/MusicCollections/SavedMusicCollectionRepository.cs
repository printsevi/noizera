using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.SavedMusicSets;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.MusicCollections;

public sealed class SavedMusicCollectionRepository(AppDbContext db) : BaseEntityRepository<SavedMusicSet>(db), ISavedMusicCollectionRepository
{
    public async Task<List<MusicCollectionCardQueryResult>> GetAllAsync(Guid userId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                mc."PublicId" as PublicId,
                mc."Title" as Title,
                mc."CollectionType" as CollectionType,
                p."PublicId" as OwnerPublicId,
                p."Name" as OwnerName,
                COUNT(mcs."Id") AS SongCount,
                TRUE as IsSaved
            FROM
                public."Users" u
            JOIN 
                public."SavedMusicCollections" smc 
                    ON u."Id" = smc."UserId"
            JOIN 
                public."MusicCollections" mc 
                    ON mc."Id" = smc."MusicSetId"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicCollectionSongs" mcs
                    ON mcs."MusicCollectionId" = mc."Id"
            WHERE 
                ((mc."CollectionType" = 'collection_album' AND mc."AlbumStatus" = 'Released')
                    OR mc."CollectionType" = 'collection_playlist')
                AND mc."IsDeleted" = false
                AND u."Id" = {userId}
            GROUP BY 
                mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."PublicId"
            Limit 300
            """;

        return await Db.Database
            .SqlQuery<MusicCollectionCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid musicCollectionId, Guid userId, CancellationToken ct)
    {
        await Db.SavedMusicCollections
            .Where(x => x.MusicSetId == musicCollectionId && x.UserId == userId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);
    }
}
