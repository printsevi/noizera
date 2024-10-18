using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Users;

public sealed record NewPasswordRule(string NewPassword, User user, IPasswordHelper PasswordHelper) : ISyncDomainRule
{
    public string ErrorMessage => $"Use another password";

    public bool Verify() => !PasswordHelper.VerifyPassword(NewPassword, user.PasswordHash, user.PasswordSalt);
}
