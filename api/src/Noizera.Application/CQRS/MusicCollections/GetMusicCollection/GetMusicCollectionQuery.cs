using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollection;

public sealed record GetMusicCollectionQuery(
    string MusicCollectionPublicId,
    Guid UserId)
    : IAuthorizeableRequest<MusicCollectionQueryResult>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetMusicCollectionQuery, MusicCollectionQueryResult>
    {
        public async Task<MusicCollectionQueryResult> Handle([NotNull] GetMusicCollectionQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetMusicCollectionAsync(request.MusicCollectionPublicId, request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            return result;
        }
    }
}
