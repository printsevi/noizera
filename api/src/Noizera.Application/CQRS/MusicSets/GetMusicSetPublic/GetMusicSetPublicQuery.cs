using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.GetMusicSetPublic;

public sealed record GetMusicSetPublicQuery(
    string MusicSetPublicId)
    : IRequest<MusicSetQueryResult>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetMusicSetPublicQuery, MusicSetQueryResult>
    {
        public async Task<MusicSetQueryResult> Handle([NotNull] GetMusicSetPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetMusicSetAsync(request.MusicSetPublicId, null, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Collection not found", ErrorType.NotFound);

            return result;
        }
    }
}
