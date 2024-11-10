using FluentValidation;

namespace Noizera.Application.CQRS.Songs.AddSongToFavourites;

public sealed class AddSongToFavouritesValidator : AbstractValidator<AddSongToFavouritesCommand>
{
    public AddSongToFavouritesValidator()
    {
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
