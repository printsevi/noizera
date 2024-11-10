using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.UpdateMusicSetTitle;

public sealed class UpdateMusicSetTitleValidator : AbstractValidator<UpdateMusicSetTitleCommand>
{
    public UpdateMusicSetTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle).MaximumLength(500);
        _ = RuleFor(x => x.MusicSetId).NotEmpty();
    }
}
