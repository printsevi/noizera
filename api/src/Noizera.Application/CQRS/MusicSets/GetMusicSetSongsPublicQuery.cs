using MediatR;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record GetMusicSetSongsPublicQuery(
    string MusicSetPublicId)
    : IRequest<List<MusicSetSongResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMusicSetSongsPublicQuery, List<MusicSetSongResult>>
    {
        public async Task<List<MusicSetSongResult>> Handle([NotNull] GetMusicSetSongsPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Mpeg, null, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
