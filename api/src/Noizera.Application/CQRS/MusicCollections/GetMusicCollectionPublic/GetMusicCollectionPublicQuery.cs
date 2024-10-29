using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollectionPublic;

public sealed record GetMusicCollectionPublicQuery(
    string MusicCollectionPublicId)
    : IRequest<MusicCollectionQueryResult>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetMusicCollectionPublicQuery, MusicCollectionQueryResult>
    {
        public async Task<MusicCollectionQueryResult> Handle([NotNull] GetMusicCollectionPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetMusicCollectionAsync(request.MusicCollectionPublicId, null, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            return result;
        }
    }
}
