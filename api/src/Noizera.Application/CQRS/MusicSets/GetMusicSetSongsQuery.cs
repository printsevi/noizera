using MediatR;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record GetMusicSetSongsQuery(
    Guid UserId,
    string MusicSetPublicId,
    string AudioType)
    : IAuthorizeableRequest<List<MusicSetSongResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMusicSetSongsQuery, List<MusicSetSongResult>>
    {
        public async Task<List<MusicSetSongResult>> Handle([NotNull] GetMusicSetSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = request.AudioType == "audio/flac"
                ? await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Flac, request.UserId, cancellationToken).ConfigureAwait(false)
                : await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Mpeg, request.UserId, cancellationToken).ConfigureAwait(false);
            return new(songs);
        }
    }
}
