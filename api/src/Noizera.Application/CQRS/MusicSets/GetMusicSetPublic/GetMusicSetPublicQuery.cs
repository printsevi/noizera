using System.Diagnostics.CodeAnalysis;
using MediatR;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.MusicSets.GetMusicSetPublic;

public sealed record GetMusicSetPublicQuery(
    string MusicSetPublicId)
    : IRequest<MusicSetCardQueryResult>
{
    public sealed class Handler(
        AppDbContext db,
        IMusicSetRepository musicSetRepository)
        : IRequestHandler<GetMusicSetPublicQuery, MusicSetCardQueryResult>
    {
        public async Task<MusicSetCardQueryResult> Handle([NotNull] GetMusicSetPublicQuery request, CancellationToken cancellationToken)
        {
            var musicSet = await musicSetRepository.GetMusicSetAsync(request.MusicSetPublicId, null, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            var credits = await db.GetAlbumsCreditsAsync([musicSet.PublicId], cancellationToken).ConfigureAwait(false);
            var result = new MusicSetCardQueryResult(
                musicSet.PublicId,
                musicSet.Title,
                musicSet.CollectionType,
                musicSet.ReleaseDate,
                musicSet.IsSaved,
                musicSet.OwnerUsername,
                musicSet.OwnerName,
                musicSet.OwnerProfileType,
                musicSet.SongCount,
                credits
            );

            return result;
        }
    }
}
