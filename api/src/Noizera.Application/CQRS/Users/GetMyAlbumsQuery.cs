using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record GetMyAlbumsQuery(Guid UserId)
    : IAuthorizeableRequest<List<GetMyAlbumsResponse>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMyAlbumsQuery, List<GetMyAlbumsResponse>>
    {
        public async Task<List<GetMyAlbumsResponse>> Handle([NotNull] GetMyAlbumsQuery request, CancellationToken cancellationToken)
        {
            var result = await db.Albums
                .Where(x => x.OwnerId == request.UserId && x.AlbumStatus != AlbumStatus.Draft)
                .Select(x => new GetMyAlbumsResponse(
                    x.PublicId,
                    x.Title,
                    x.AlbumStatus.ToString(),
                    x.AlbumReleaseDate!.Value
                ))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
    }
}

public sealed record GetMyAlbumsResponse(
    string PublicId,
    string Title,
    string Status,
    DateOnly ReleaseDate);
