using FluentValidation;

namespace Noizera.Application.CQRS.Profiles.GetFollowings;

public sealed class GetFollowingsValidator : AbstractValidator<GetFollowingsQuery>
{
    public GetFollowingsValidator()
    {
        _ = RuleFor(x => x.ProfilePublicId).NotEmpty();
    }
}
