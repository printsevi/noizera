using FluentValidation;

namespace Noizera.Application.CQRS.SavedMusicSets.DeleteSavedMusicCollection;

public sealed class DeleteSavedMusicSetValidator : AbstractValidator<DeleteSavedMusicSetCommand>
{
    public DeleteSavedMusicSetValidator()
    {
        _ = RuleFor(x => x.MusicSetPublicId).NotEmpty();
    }
}
