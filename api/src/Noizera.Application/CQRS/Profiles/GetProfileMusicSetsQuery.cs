using FluentValidation;
using MediatR;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles;

public sealed record GetProfileMusicSetsQuery(
    string ProfileUsername,
    Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(AppDbContext db)
        : IRequestHandler<GetProfileMusicSetsQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetProfileMusicSetsQuery request, CancellationToken cancellationToken)
            => await db.GetProfileMusicSetsAsync(request.ProfileUsername, request.UserId, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class GetProfileMusicSetsValidator : AbstractValidator<GetProfileMusicSetsQuery>
{
    public GetProfileMusicSetsValidator()
    {
        _ = RuleFor(x => x.ProfileUsername).NotEmpty();
    }
}
