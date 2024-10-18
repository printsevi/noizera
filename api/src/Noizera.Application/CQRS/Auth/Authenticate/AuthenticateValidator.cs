using FluentValidation;

namespace Noizera.Application.CQRS.Auth.Authenticate;

public sealed class AuthenticateValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateValidator()
    {
        _ = RuleFor(x => x.EmailOrUsername).NotEmpty();
        _ = RuleFor(x => x.Code).NotEmpty();
    }
}
