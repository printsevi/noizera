using MediatR;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.GetAudioStream;

public sealed record GetAudioStreamQuery(
    string SongPublicId,
    string Range,
    long FileLength,
    string AudioType) : IRequest<AudioStreamResult>
{
    public sealed class GetAudioStreamQueryHandler(
        IAudioFileService audioFileService)
        : IRequestHandler<GetAudioStreamQuery, AudioStreamResult>
    {
        public async Task<AudioStreamResult> Handle([NotNull] GetAudioStreamQuery request, CancellationToken cancellationToken)
        {
            var result = await audioFileService.GetAudioFileAsStream(request.SongPublicId, request.Range, request.FileLength, request.AudioType, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
