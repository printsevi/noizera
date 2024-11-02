using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicCollections.GetSavedMusicCollections;

public sealed record GetSavedMusicCollectionsQuery(
    Guid UserId)
    : IAuthorizeableRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        ISavedMusicCollectionRepository savedMusicCollectionRepository)
        : IRequestHandler<GetSavedMusicCollectionsQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle([NotNull] GetSavedMusicCollectionsQuery request, CancellationToken cancellationToken)
        {
            var result = await savedMusicCollectionRepository.GetAllAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
