using FluentValidation;

namespace Noizera.Application.CQRS.Songs.AddPlay;

public sealed class AddPlayValidator : AbstractValidator<AddPlayCommand>
{
    public AddPlayValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.SongPublicId).NotEmpty();
    }
}
