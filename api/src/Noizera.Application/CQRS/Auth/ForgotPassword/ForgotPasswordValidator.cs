using FluentValidation;

namespace Noizera.Application.CQRS.Auth.ForgotPassword;

public sealed class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        _ = RuleFor(x => x.EmailOrUsername).NotEmpty();
    }
}
