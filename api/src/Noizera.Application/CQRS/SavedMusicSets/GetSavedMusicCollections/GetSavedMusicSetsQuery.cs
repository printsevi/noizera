using MediatR;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets.GetSavedMusicCollections;

public sealed record GetSavedMusicSetsQuery(
    Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        ISavedMusicSetRepository savedMusicSetRepository)
        : IRequestHandler<GetSavedMusicSetsQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetSavedMusicSetsQuery request, CancellationToken cancellationToken)
        {
            var result = await savedMusicSetRepository.GetAllAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
