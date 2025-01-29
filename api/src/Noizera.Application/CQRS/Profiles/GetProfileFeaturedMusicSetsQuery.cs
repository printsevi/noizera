using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Profiles;

public sealed record GetProfileFeaturedMusicSetsQuery(
    string ProfileUsername,
    Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(AppDbContext db)
        : IRequestHandler<GetProfileFeaturedMusicSetsQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetProfileFeaturedMusicSetsQuery request, CancellationToken cancellationToken)
        {
            var profileSets = await db.GetProfileFeaturedMusicSetsAsync(request.ProfileUsername, request.UserId, cancellationToken).ConfigureAwait(false);
            var albumPublicIds = profileSets.Select(x => x.PublicId).Distinct();
            var credits = await db.GetAlbumsCreditsAsync(albumPublicIds, cancellationToken).ConfigureAwait(false);
            var result = profileSets.Select(x => new MusicSetCardQueryResult(
                x.PublicId,
                x.Title,
                x.CollectionType,
                x.ReleaseDate,
                x.IsSaved,
                x.OwnerUsername,
                x.OwnerName,
                x.OwnerProfileType,
                x.SongCount,
                credits.Where(c => c.MusicSetPublicId == x.PublicId)
            ));

            return result.ToList();
        }
    }
}

public sealed class GetProfileFeaturedMusicSetsValidator : AbstractValidator<GetProfileFeaturedMusicSetsQuery>
{
    public GetProfileFeaturedMusicSetsValidator()
    {
        _ = RuleFor(x => x.ProfileUsername).NotEmpty();
    }
}
