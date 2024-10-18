using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.AddAlbumSong;

public sealed class AddAlbumSongValidator : AbstractValidator<AddAlbumSongCommand>
{
    public AddAlbumSongValidator() => RuleFor(x => x.AlbumId).NotEmpty();
}
