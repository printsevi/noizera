using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.SaveMusicCollection;

public sealed class SaveMusicCollectionValidator : AbstractValidator<SaveMusicCollectionCommand>
{
    public SaveMusicCollectionValidator()
    {
        _ = RuleFor(x => x.MusicCollectionId).NotEmpty();
    }
}
