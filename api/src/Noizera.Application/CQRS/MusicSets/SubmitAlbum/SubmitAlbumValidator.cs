using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.SubmitAlbum;

public sealed class SubmitAlbumValidator : AbstractValidator<SubmitAlbumCommand>
{
    public SubmitAlbumValidator() => RuleFor(x => x.AlbumId).NotEmpty();
}
