using MediatR;
using Noizera.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.GetAudioStream;

public sealed record GetAudioStreamQuery(
    string SongPublicId) : IRequest<Stream>
{
    public sealed class GetAudioStreamQueryHandler(
        IAudioMongoService audioService)
        : IRequestHandler<GetAudioStreamQuery, Stream>
    {
        public async Task<Stream> Handle([NotNull] GetAudioStreamQuery request, CancellationToken cancellationToken) 
            => await audioService.GetAudioAsStreamAsync(request.SongPublicId, cancellationToken).ConfigureAwait(false);
    }
}
