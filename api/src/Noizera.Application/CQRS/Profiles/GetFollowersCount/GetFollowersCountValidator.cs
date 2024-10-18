using FluentValidation;

namespace Noizera.Application.CQRS.Profiles.GetFollowersCount;

public sealed class GetFollowersCountValidator : AbstractValidator<GetFollowersCountQuery>
{
    public GetFollowersCountValidator()
    {
        _ = RuleFor(x => x.ProfilePublicId).NotEmpty();
    }
}
