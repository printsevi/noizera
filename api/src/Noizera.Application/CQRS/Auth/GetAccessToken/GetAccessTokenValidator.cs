using FluentValidation;

namespace Noizera.Application.CQRS.Auth.GetAccessToken;

public sealed class GetAccessTokenValidator : AbstractValidator<GetAccessTokenQuery>
{
    public GetAccessTokenValidator()
    {
        _ = RuleFor(x => x.RefreshToken).NotEmpty();
        _ = RuleFor(x => x.ExpiredAccessToken).NotEmpty();
    }
}
