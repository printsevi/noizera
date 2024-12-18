using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record GetAlbumCreditsQuery(
    string AlbumPublicId)
    : IRequest<List<GetAlbumCreditsResponse>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetAlbumCreditsQuery, List<GetAlbumCreditsResponse>>
    {
        public async Task<List<GetAlbumCreditsResponse>> Handle([NotNull] GetAlbumCreditsQuery request, CancellationToken cancellationToken)
        {
            FormattableString sql = $"""
                SELECT 
                    p."ProfileType" as ProfileType,
                    p."Username" as Username,
                    p."Name" as Name,
                    ac."ProfileName" as ProfileName
                FROM 
                    public."MusicSets" mc
                JOIN 
                    public."AlbumCredits" ac
                        ON ac."AlbumId" = mc."Id"
                JOIN 
                    public."Profiles" p
                        ON p."Id" = ac."ProfileId"
                WHERE 
                    mc."PublicId" = UPPER({request.AlbumPublicId})
                        AND mc."CollectionType" = 'collection_album' 
                        AND mc."AlbumStatus" = 'Released'
                        AND mc."IsDeleted" = FALSE
             """;

            var result = await db.Database
                .SqlQuery<GetAlbumCreditsResponse>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result
                ?? throw new AppException("Collection not found", ErrorType.NotFound);
        }
    }
}

public sealed class GetAlbumCreditsQueryValidator : AbstractValidator<GetAlbumCreditsQuery>
{
    public GetAlbumCreditsQueryValidator()
    {
        _ = RuleFor(x => x.AlbumPublicId).NotEmpty();
    }
}

public record GetAlbumCreditsResponse(
    string? Username,
    string? Name,
    string? ProfileName,
    string? ProfileType);
