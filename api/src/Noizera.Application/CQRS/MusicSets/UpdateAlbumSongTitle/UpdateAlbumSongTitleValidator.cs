using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.UpdateAlbumSongTitle;

public sealed class UpdateAlbumSongTitleValidator : AbstractValidator<UpdateAlbumSongTitleCommand>
{
    public UpdateAlbumSongTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle)
            .MinimumLength(0)
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
        _ = RuleFor(x => x.AlbumId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
