using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.AddAlbumSong;

public sealed class AddAlbumSongValidator : AbstractValidator<AddAlbumSongCommand>
{
    public AddAlbumSongValidator() => RuleFor(x => x.AlbumId).NotEmpty();
}
