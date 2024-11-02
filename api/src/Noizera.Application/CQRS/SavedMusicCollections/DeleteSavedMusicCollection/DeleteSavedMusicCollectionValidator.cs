using FluentValidation;

namespace Noizera.Application.CQRS.SavedMusicCollections.DeleteSavedMusicCollection;

public sealed class DeleteSavedMusicCollectionValidator : AbstractValidator<DeleteSavedMusicCollectionCommand>
{
    public DeleteSavedMusicCollectionValidator()
    {
        _ = RuleFor(x => x.MusicCollectionPublicId).NotEmpty();
    }
}
