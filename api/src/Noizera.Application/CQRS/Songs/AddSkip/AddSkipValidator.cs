using FluentValidation;

namespace Noizera.Application.CQRS.Songs.AddSkip;

public sealed class AddSkipValidator : AbstractValidator<AddSkipCommand>
{
    public AddSkipValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.SongId).NotEmpty();
    }
}
