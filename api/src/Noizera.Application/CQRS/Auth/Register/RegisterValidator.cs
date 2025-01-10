using FluentValidation;
using Noizera.Common.Domain.Common;

namespace Noizera.Application.CQRS.Auth.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        _ = RuleFor(x => x.Email).NotEmpty().EmailAddress();
        _ = RuleFor(x => x.ProfileUserName).NotEmpty().MinimumLength(2).MaximumLength(Constants.UsernameMaxLength);
        _ = RuleFor(x => x.Password)
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
            .Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
            .Matches(@"\d").WithMessage("Password must contain one or more digits.")
            .Matches(@"[~`¿¡!#$%\^&*€£@+÷=\-\[\]\\';,/{}\(\)|\\"":<>\?\.\_]").WithMessage("Password must contain one or more special characters.")
            //.Matches("^[^£# “”]*$").WithMessage("'{PropertyName}' must not contain the following characters £ # “” or spaces.")
            .WithMessage("Password is not strong enough");

    }
}
