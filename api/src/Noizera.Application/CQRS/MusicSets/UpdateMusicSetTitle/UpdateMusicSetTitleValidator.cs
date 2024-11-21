using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.UpdateMusicSetTitle;

public sealed class UpdateMusicSetTitleValidator : AbstractValidator<UpdateMusicSetTitleCommand>
{
    public UpdateMusicSetTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle)
            .MinimumLength(0)
            .MaximumLength(100)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
        _ = RuleFor(x => x.MusicSetId).NotEmpty();
    }
}
