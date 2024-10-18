using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollection;

public sealed record GetMusicCollectionQuery(
    string MusicCollectionPublicId,
    Guid? UserId)
    : IRequest<GetMusicCollectionResponse>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetMusicCollectionQuery, GetMusicCollectionResponse>
    {
        public async Task<GetMusicCollectionResponse> Handle([NotNull] GetMusicCollectionQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetMusicCollectionAsync(request.MusicCollectionPublicId, request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            return new(result.Title);
        }
    }
}
