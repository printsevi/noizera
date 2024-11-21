using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetArtists;

public sealed record GetArtistsQuery(string Text, Guid UserId)
    : IAuthorizeableRequest<List<GetArtistsResponse>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetArtistsQuery, List<GetArtistsResponse>>
    {
        public async Task<List<GetArtistsResponse>> Handle([NotNull] GetArtistsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new([]);
            }

            FormattableString sql = $"""
                SELECT
                    p."Id" as ArtistId,
                    p."Name" as Name,
                    p."PublicId" as PublicId,
                    p."Username" as Username,
                    SIMILARITY(p."Name", {request.Text}) AS Score
                FROM public."Profiles" p
                WHERE
                    p."ProfileType" = 'Artist'
                    AND p."UserId" != {request.UserId}
                    AND SIMILARITY(p."Name", {request.Text}) > 0.1
                ORDER BY Name, Score DESC
                LIMIT 10;
                """;

            var result = await db.Database.SqlQuery<GetArtistsResponse>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
