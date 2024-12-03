using FluentValidation;
using MediatR;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles;

public sealed record GetProfileMusicSetsPublicQuery(
    string ProfileUsername) : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(AppDbContext db)
        : IRequestHandler<GetProfileMusicSetsPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetProfileMusicSetsPublicQuery request, CancellationToken cancellationToken)
            => await db.GetProfileMusicSetsAsync(request.ProfileUsername, null, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetProfileMusicSetsPublicValidator : AbstractValidator<GetProfileMusicSetsPublicQuery>
{
    public GetProfileMusicSetsPublicValidator()
    {
        _ = RuleFor(x => x.ProfileUsername).NotEmpty();
    }
}
