using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.UpdateAlbumSongTitle;

public sealed class UpdateAlbumSongTitleValidator : AbstractValidator<UpdateAlbumSongTitleCommand>
{
    public UpdateAlbumSongTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle).NotEmpty();
        _ = RuleFor(x => x.AlbumId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
