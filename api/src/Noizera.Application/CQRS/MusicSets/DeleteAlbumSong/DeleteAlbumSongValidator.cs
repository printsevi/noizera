using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.DeleteAlbumSong;

public sealed class DeleteAlbumSongValidator : AbstractValidator<DeleteAlbumSongCommand>
{
    public DeleteAlbumSongValidator()
    {
        _ = RuleFor(x => x.AlbumId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
