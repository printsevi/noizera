using FluentValidation;

namespace Noizera.Application.CQRS.Profiles.Follow;

public sealed class FollowValidator : AbstractValidator<FollowCommand>
{
    public FollowValidator()
    {
        _ = RuleFor(x => x.ProfileId).NotEmpty();
    }
}
