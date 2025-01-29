using FluentValidation;
using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles;

public sealed record GetProfileFeaturedMusicSetsPublicQuery(
    string ProfileUsername) : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(AppDbContext db)
        : IRequestHandler<GetProfileFeaturedMusicSetsPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetProfileFeaturedMusicSetsPublicQuery request, CancellationToken cancellationToken)
        {
            var profileSets = await db.GetProfileFeaturedMusicSetsAsync(request.ProfileUsername, null, cancellationToken).ConfigureAwait(false);
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

public sealed class GetProfileFeaturedMusicSetsPublicValidator : AbstractValidator<GetProfileFeaturedMusicSetsPublicQuery>
{
    public GetProfileFeaturedMusicSetsPublicValidator()
    {
        _ = RuleFor(x => x.ProfileUsername).NotEmpty();
    }
}
