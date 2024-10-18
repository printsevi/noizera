using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateUsername;

public sealed class UpdateUsernameValidator : AbstractValidator<UpdateUsernameCommand>
{
    public UpdateUsernameValidator()
    {
        _ = RuleFor(x => x.NewUsername).MinimumLength(2).MaximumLength(30);
    }
}
