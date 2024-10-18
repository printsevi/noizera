using FluentValidation;

namespace Noizera.Application.CQRS.Songs.AddSongToPlaylist;

public sealed class AddSongToPlaylistValidator : AbstractValidator<AddSongToPlaylistCommand>
{
    public AddSongToPlaylistValidator()
    {
        _ = RuleFor(x => x.PlaylistId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
