using FluentValidation;

namespace Noizera.Application.CQRS.SavedMusicSets.AddSavedMusicCollection;

public sealed class AddSavedMusicSetValidator : AbstractValidator<AddSavedMusicSetCommand>
{
    public AddSavedMusicSetValidator()
    {
        _ = RuleFor(x => x.MusicSetPublicId).NotEmpty();
    }
}
