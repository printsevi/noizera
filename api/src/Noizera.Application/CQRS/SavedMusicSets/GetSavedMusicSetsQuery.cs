using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets;

public sealed record GetSavedMusicSetsQuery(
    Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetSavedMusicSetsQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetSavedMusicSetsQuery request, CancellationToken cancellationToken)
        {
            FormattableString sql = $"""
                WITH Results AS (
                    SELECT 
                        mc."PublicId" as PublicId,
                        mc."Title" as Title,
                        mc."CollectionType" as CollectionType,
                        p."Username" as OwnerUsername,
                        p."Name" as OwnerName,
                        COUNT(mcs."Id") AS SongCount,
                        FALSE as IsSaved
                    FROM
                        public."Users" u
                    JOIN 
                        public."MusicSets" mc 
                            ON mc."OwnerId" = u."Id"
                    LEFT JOIN 
                        public."Profiles" p
                            ON p."UserId" = u."Id"
                    LEFT JOIN 
                        public."MusicSetSongs" mcs
                            ON mcs."MusicSetId" = mc."Id"
                    WHERE 
                        mc."CollectionType" = 'collection_playlist'
                        AND mc."IsDeleted" = false
                        AND u."Id" = {request.UserId}
                    GROUP BY 
                        mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username"
        
                    UNION ALL

                    SELECT 
                        mc."PublicId" as PublicId,
                        mc."Title" as Title,
                        mc."CollectionType" as CollectionType,
                        p."Username" as OwnerUsername,
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
                        AND u."Id" = {request.UserId}
                    GROUP BY 
                        mc."PublicId", mc."Title", mc."CollectionType", p."Name", p."Username"
                )
                SELECT *
                FROM Results
                ORDER BY 
                    CASE WHEN CollectionType = 'collection_playlist' THEN 1 ELSE 2 END ASC, 
                    IsSaved DESC, 
                    PublicId DESC
                LIMIT 200
            """;

            var result = await db.Database
                .SqlQuery<MusicSetCardQueryResult>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
