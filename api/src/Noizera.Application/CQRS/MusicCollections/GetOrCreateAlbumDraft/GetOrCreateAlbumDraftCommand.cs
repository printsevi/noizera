using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Songs;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetOrCreateAlbumDraft;

public sealed record GetOrCreateAlbumDraftCommand(
    Guid UserId)
    : IAuthorizeableRequest<GetOrCreateAlbumDraftResponse>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IAlbumRepository albumRepository,
        ISongRepository songRepository)
        : IRequestHandler<GetOrCreateAlbumDraftCommand, GetOrCreateAlbumDraftResponse>
    {
        public async Task<GetOrCreateAlbumDraftResponse> Handle([NotNull] GetOrCreateAlbumDraftCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("User not found", ErrorType.NotFound);

            var albumDraft = await albumRepository.GetLatestDraftAsync(user.Id, cancellationToken).ConfigureAwait(false);
            if (albumDraft is null)
            {
                albumDraft = Album.Create(user);
                await albumRepository.InsertAsync(albumDraft, cancellationToken).ConfigureAwait(false);

                Song song = Song.Create(user, albumDraft);
                await songRepository.InsertAsync(song, cancellationToken).ConfigureAwait(false);
            }

            var songs = albumDraft.MusicCollectionSongs
                .Select(x => new GetOrCreateAlbumDraftSongResponse(
                    x.Song.Id, x.Song.Title, x.Song.PublicId, x.Song.OriginalFileName, x.Song.OriginalContentLength, x.Song.OriginalContentType, x.Sequence));

            var credits = albumDraft.Credits
                .Select(x => new GetOrCreateAlbumDraftCreditResponse(
                    x.Profile!.Id, x.Profile!.DisplayName));

            return new(
                albumDraft.Id,
                albumDraft.PublicId,
                albumDraft.Title,
                albumDraft.Description,
                albumDraft.CoverImageBucketName,
                albumDraft.CoverImageOriginalName,
                user.Profile!.DisplayName,
                user.Profile.ProfileType.ToString(),
                songs,
                credits);
        }
    }
}
