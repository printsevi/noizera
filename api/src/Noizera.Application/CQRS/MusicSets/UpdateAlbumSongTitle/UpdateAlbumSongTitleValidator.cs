using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.UpdateAlbumSongTitle;

public sealed class UpdateAlbumSongTitleValidator : AbstractValidator<UpdateAlbumSongTitleCommand>
{
    public UpdateAlbumSongTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle).MinimumLength(0).MaximumLength(500);
        _ = RuleFor(x => x.AlbumId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
