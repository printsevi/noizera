using FluentValidation;

namespace Noizera.Application.CQRS.Profiles.GetFollowingsCount;

public sealed class GetFollowingsCountValidator : AbstractValidator<GetFollowingsCountQuery>
{
    public GetFollowingsCountValidator()
    {
        _ = RuleFor(x => x.ProfilePublicId).NotEmpty();
    }
}
