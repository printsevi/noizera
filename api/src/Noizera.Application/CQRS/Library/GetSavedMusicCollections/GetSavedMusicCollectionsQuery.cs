using MediatR;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Library.GetSavedMusicCollections;

public sealed record GetSavedMusicCollectionsQuery(
    Guid UserId)
    : IAuthorizeableRequest<GetSavedMusicCollectionsResponse>
{
    public sealed class Handler(
        ISavedMusicCollectionRepository savedMusicCollectionRepository)
        : IRequestHandler<GetSavedMusicCollectionsQuery, GetSavedMusicCollectionsResponse>
    {
        public async Task<GetSavedMusicCollectionsResponse> Handle([NotNull] GetSavedMusicCollectionsQuery request, CancellationToken cancellationToken)
        {
            var result = await savedMusicCollectionRepository.GetAllAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
