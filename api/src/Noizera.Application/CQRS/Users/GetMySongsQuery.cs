using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Noizera.Application.CQRS.Users;

public sealed record GetMySongsQuery(Guid UserId)
    : IAuthorizeableRequest<List<GetMySongsResponse>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMySongsQuery, List<GetMySongsResponse>>
    {
        public async Task<List<GetMySongsResponse>> Handle([NotNull] GetMySongsQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Songs
                .Where(x => x.OwnerId == request.UserId && x.IsPublic)
                .Select(x => new GetMySongsResponse(
                    x.PublicId,
                    x.Title!,
                    x.Streams.Count,
                    x.MusicSetSongs.Select(ms => ms.MusicSet).OfType<Playlist>().Count(p => p.PlaylistTag == PlaylistConstants.FavouritesPlaylistTag)
                ))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}

public sealed record GetMySongsResponse(
    string PublicId,
    string Title,
    int StreamCount,
    int LikeCount);
