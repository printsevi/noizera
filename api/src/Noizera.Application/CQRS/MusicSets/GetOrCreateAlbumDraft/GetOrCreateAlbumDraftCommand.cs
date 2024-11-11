using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.GetOrCreateAlbumDraft;

public sealed record GetOrCreateAlbumDraftCommand(
    Guid UserId)
    : IAuthorizeableRequest<GetOrCreateAlbumDraftResponse>
{
    public sealed class Handler(
        AppDbContext db,
        IHashGenerator hashGenerator)
        : IRequestHandler<GetOrCreateAlbumDraftCommand, GetOrCreateAlbumDraftResponse>
    {
        public async Task<GetOrCreateAlbumDraftResponse> Handle([NotNull] GetOrCreateAlbumDraftCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Include(u => u.Profile)
                .Include(u => u.Songs)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            var albumDraft = await db.Albums
                .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                    .ThenInclude(i => i.Song)
                        .ThenInclude(s => s.Credits)
                            .ThenInclude(c => c.Profile)
                .FirstOrDefaultAsync(x => x.OwnerId == request.UserId && x.AlbumStatus == AlbumStatus.Draft, cancellationToken)
                .ConfigureAwait(false);

            if (albumDraft is null)
            {
                albumDraft = await Album.NewAsync(user, hashGenerator, cancellationToken).ConfigureAwait(false);
                await db.InsertAsync(albumDraft, cancellationToken).ConfigureAwait(false);

                var song = await Song.NewAsync(user, albumDraft, hashGenerator, cancellationToken).ConfigureAwait(false);
                await db.InsertAsync(song, cancellationToken).ConfigureAwait(false);
            }

            var songs = albumDraft.MusicSetSongs
                .Select(x => new GetOrCreateAlbumDraftSongResponse(
                    x.Song.Id, 
                    x.Song.Title, 
                    x.Song.PublicId,
                    x.Song.OriginalFileName, 
                    x.Song.OriginalContentLength, 
                    x.Song.OriginalContentType,
                    x.Sequence,
                    x.Song.Credits.Select(x => new GetOrCreateAlbumDraftCreditResponse(
                        x.Id, 
                        x.Profile?.PublicId ?? null,
                        x.Profile?.Id ?? null,
                        x.ProfileName
                    ))));

            return new(
                albumDraft.Id,
                albumDraft.PublicId,
                albumDraft.Title,
                albumDraft.Description,
                albumDraft.AlbumReleaseDate,
                albumDraft.CoverImageS3Folder,
                albumDraft.CoverImageOriginalName,
                user.Profile.DisplayName,
                user.Profile.ProfileType.ToString(),
                songs);
        }
    }
}
