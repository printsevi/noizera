using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Users;

public sealed record NewPasswordRule(string NewPassword, User User, IPasswordHelper PasswordHelper) : ISyncDomainRule
{
    public string ErrorMessage => $"Use another password";

    public bool Verify() => !PasswordHelper.VerifyPassword(NewPassword, User.PasswordHash, User.PasswordSalt);
}
