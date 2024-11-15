using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.SavedMusicSets;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.MusicSets;

public sealed class SavedMusicSetRepository(AppDbContext db) : BaseEntityRepository<SavedMusicSet>(db), ISavedMusicSetRepository
{
    public async Task<List<MusicSetCardQueryResult>> GetAllAsync(Guid userId, CancellationToken ct)
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
                public."SavedMusicSets" smc 
                    ON u."Id" = smc."UserId"
            JOIN 
                public."MusicSets" mc 
                    ON mc."Id" = smc."MusicSetId"
            LEFT JOIN 
                public."Profiles" p
                    ON p."UserId" = u."Id"
            LEFT JOIN 
                public."MusicSetSongs" mcs
                    ON mcs."MusicSetId" = mc."Id"
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
            .SqlQuery<MusicSetCardQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid MusicSetId, Guid userId, CancellationToken ct)
    {
        await Db.SavedMusicSets
            .Where(x => x.MusicSetId == MusicSetId && x.UserId == userId)
            .ExecuteDeleteAsync(ct).ConfigureAwait(false);
    }
}
