using FluentValidation;

namespace Noizera.Application.CQRS.SavedMusicCollections.AddSavedMusicCollection;

public sealed class AddSavedMusicCollectionValidator : AbstractValidator<AddSavedMusicCollectionCommand>
{
    public AddSavedMusicCollectionValidator()
    {
        _ = RuleFor(x => x.MusicCollectionPublicId).NotEmpty();
    }
}
