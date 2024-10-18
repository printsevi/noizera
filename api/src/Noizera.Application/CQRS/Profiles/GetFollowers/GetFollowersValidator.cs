using FluentValidation;

namespace Noizera.Application.CQRS.Profiles.GetFollowers;

public sealed class GetFollowersValidator : AbstractValidator<GetFollowersQuery>
{
    public GetFollowersValidator()
    {
        _ = RuleFor(x => x.ProfilePublicId).NotEmpty();
    }
}
