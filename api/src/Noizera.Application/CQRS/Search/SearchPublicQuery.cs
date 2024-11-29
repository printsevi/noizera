using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Search;

public sealed record SearchPublicQuery(string SearchQuery)
    : IRequest<List<SearchQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<SearchPublicQuery, List<SearchQueryResult>>
    {
        public async Task<List<SearchQueryResult>> Handle([NotNull] SearchPublicQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                return [];
            }

            FormattableString sql = $"""
                WITH Results AS (
                    SELECT 
                        p."Name" as Title, 
                        p."Username" as PublicId,
                        p."PublicId" as ImageId,
                        'Profiles' as Category,
                        SIMILARITY(p."Name", {request.SearchQuery}) AS Score
                    FROM 
                        public."Profiles" p
                    WHERE 
                        (p."ProfileType" = 'Artist' OR p."ProfileType" = 'Label')
                        AND p."IsDeleted" = FALSE
                        AND SIMILARITY(p."Name", {request.SearchQuery}) > 0.1

                    UNION ALL

                    SELECT 
                        a."Title" as Title, 
                        a."PublicId" as PublicId,
                        a."PublicId" as ImageId,
                        'Albums' as Category,
                        SIMILARITY(s."Title", {request.SearchQuery}) AS Score
                    FROM 
                        public."Songs" s
                    JOIN 
                        public."MusicSets" a
                            ON s."AlbumId" = a."Id" 
                    WHERE 
                        s."IsPublic" = TRUE 
                        AND a."IsDeleted" = FALSE
                        AND SIMILARITY(s."Title", {request.SearchQuery}) > 0.1

                    UNION ALL

                    SELECT 
                        mc."Title" as Title, 
                        mc."PublicId" as PublicId,
                        mc."PublicId" as ImageId,
                        'Albums' as Category,
                        SIMILARITY(mc."Title", {request.SearchQuery}) AS Score
                    FROM 
                        public."MusicSets" mc
                    WHERE 
                        mc."CollectionType" = 'collection_album'
                        AND mc."AlbumStatus" = 'Released'
                        AND mc."IsDeleted" = FALSE
                        AND SIMILARITY(mc."Title", {request.SearchQuery}) > 0.1
                )
                SELECT DISTINCT ON (PublicId) *
                FROM Results
                ORDER BY PublicId, Score DESC
                LIMIT 20;
            """;

            var result = await db.Database.SqlQuery<SearchQueryResult>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}

public record SearchQueryResult(
    string Category,
    string Title,
    string PublicId,
    string ImageId);
