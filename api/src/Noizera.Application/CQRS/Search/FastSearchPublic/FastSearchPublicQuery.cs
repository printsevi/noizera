using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Search.FastSearchPublic;

public sealed record FastSearchPublicQuery(string SearchQuery)
    : IRequest<List<FastSearchQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<FastSearchPublicQuery, List<FastSearchQueryResult>>
    {
        public async Task<List<FastSearchQueryResult>> Handle([NotNull] FastSearchPublicQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                return [];
            }

            FormattableString sql = $"""
                WITH Results AS (
                    SELECT 
                        p."Name" as Value, 
                        SIMILARITY(p."Name", {request.SearchQuery}) AS Score
                    FROM 
                        public."Profiles" p
                    WHERE 
                        (p."ProfileType" = 'Artist' OR p."ProfileType" = 'Label')
                        AND SIMILARITY(p."Name", {request.SearchQuery}) > 0.1

                    UNION ALL

                    SELECT 
                        s."Title" as Value, 
                        SIMILARITY(s."Title", {request.SearchQuery}) AS Score
                    FROM 
                        public."Songs" s
                    WHERE 
                        s."IsPublic" = TRUE 
                        AND SIMILARITY(s."Title", {request.SearchQuery}) > 0.1

                    UNION ALL

                    SELECT 
                        mc."Title" as Value, 
                        SIMILARITY(mc."Title", {request.SearchQuery}) AS Score
                    FROM 
                        public."MusicSets" mc
                    WHERE 
                        mc."CollectionType" = 'collection_album'
                        AND mc."AlbumStatus" = 'Released'
                        AND mc."IsDeleted" = FALSE
                        AND SIMILARITY(mc."Title", {request.SearchQuery}) > 0.1
                )
                SELECT DISTINCT ON (Value) *
                FROM Results
                ORDER BY Value, Score DESC
                LIMIT 10;
            """;

            var result = await db.Database.SqlQuery<FastSearchQueryResult>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
