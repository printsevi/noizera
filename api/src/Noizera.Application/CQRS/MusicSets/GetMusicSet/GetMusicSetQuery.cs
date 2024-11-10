using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.GetMusicSet;

public sealed record GetMusicSetQuery(
    string MusicSetPublicId,
    Guid UserId)
    : IAuthorizeableRequest<MusicSetQueryResult>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetMusicSetQuery, MusicSetQueryResult>
    {
        public async Task<MusicSetQueryResult> Handle([NotNull] GetMusicSetQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetMusicSetAsync(request.MusicSetPublicId, request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            return result;
        }
    }
}
