using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.DeleteAlbumSong;

public sealed class DeleteAlbumSongValidator : AbstractValidator<DeleteAlbumSongCommand>
{
    public DeleteAlbumSongValidator()
    {
        _ = RuleFor(x => x.AlbumId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
