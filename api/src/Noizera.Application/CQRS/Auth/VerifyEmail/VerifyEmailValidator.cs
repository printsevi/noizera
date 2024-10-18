using FluentValidation;

namespace Noizera.Application.CQRS.Auth.VerifyEmail;

public sealed class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailValidator()
    {
        _ = RuleFor(x => x.Email).EmailAddress();
        _ = RuleFor(x => x.Code).NotEmpty();
    }
}
