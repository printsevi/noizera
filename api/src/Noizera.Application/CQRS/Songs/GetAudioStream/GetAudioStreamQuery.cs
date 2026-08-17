using MediatR;
using Noizera.Application.CQRS.Songs.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.GetAudioStream;

/// <summary>
/// Streams a byte range of a song's audio.
/// </summary>
/// <remarks>
/// This is not an <c>IAuthorizeableRequest</c> because the endpoint serves anonymous listeners
/// the free MP3 rendition; authorization is performed by <see cref="AudioAccessGuard"/> against
/// the optional <paramref name="UserId"/> instead of by the pipeline behavior.
/// <para>
/// The file length is deliberately <b>not</b> a request parameter. It used to arrive from the
/// query string, letting a caller declare an arbitrary length and steer the range read; it is
/// now resolved from the stored song.
/// </para>
/// </remarks>
public sealed record GetAudioStreamQuery(
    string SongPublicId,
    string Range,
    string AudioType,
    Guid? UserId) : IRequest<AudioStreamResult>
{
    public sealed class GetAudioStreamQueryHandler(
        IAudioFileService audioFileService,
        AudioAccessGuard audioAccessGuard)
        : IRequestHandler<GetAudioStreamQuery, AudioStreamResult>
    {
        public async Task<AudioStreamResult> Handle([NotNull] GetAudioStreamQuery request, CancellationToken cancellationToken)
        {
            var access = await audioAccessGuard
                .AuthorizeAsync(request.SongPublicId, request.AudioType, request.UserId, cancellationToken)
                .ConfigureAwait(false);

            var result = await audioFileService
                .GetAudioFileAsStream(request.SongPublicId, request.Range, access.ContentLength, request.AudioType, cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}
