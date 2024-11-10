using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.GetCoverImage;

public sealed class GetCoverImageValidator : AbstractValidator<GetCoverImageQuery>
{
    public GetCoverImageValidator()
    {
        _ = RuleFor(x => x.MusicSetPublicId).NotEmpty();
    }
}
