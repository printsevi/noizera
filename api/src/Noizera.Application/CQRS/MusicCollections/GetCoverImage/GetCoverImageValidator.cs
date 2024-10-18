using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.GetCoverImage;

public sealed class GetCoverImageValidator : AbstractValidator<GetCoverImageQuery>
{
    public GetCoverImageValidator()
    {
        _ = RuleFor(x => x.MusicCollectionPublicId).NotEmpty();
    }
}
