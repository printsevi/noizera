using FluentValidation;
using Noizera.Application.CQRS.Profiles.GetFollowings;

namespace Noizera.Application.CQRS.Profiles.Unfollow;

public sealed class UnfollowValidator : AbstractValidator<UnfollowCommand>
{
    public UnfollowValidator()
    {
        _ = RuleFor(x => x.ProfileId).NotEmpty();
    }
}
