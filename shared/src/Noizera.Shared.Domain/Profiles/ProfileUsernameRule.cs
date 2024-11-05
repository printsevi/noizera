using Noizera.Shared.Domain.Common;
using System.Text.RegularExpressions;

namespace Noizera.Shared.Domain.Profiles;

public sealed record ProfileUsernameRule(string Username) : ISyncDomainRule
{
    public string ErrorMessage => $"Username format is wrong for {Username}";

    public bool Verify()
    {
        var regex = new Regex(@"^(?!\.)(?!.*\.$)(?!.*__)(?!.*\.\.)[a-zA-Z0-9._]{2,30}$");

        return Username == Username.ToLowerInvariant() && regex.IsMatch(Username);
    }
}
