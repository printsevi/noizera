using Noizera.Common.Domain.Common;
using System.Text.RegularExpressions;

namespace Noizera.Common.Domain.Profiles;

public sealed record ProfileUsernameRule(string Username) : ISyncDomainRule
{
    public string ErrorMessage => $"Username format is wrong for {Username}";

    public bool Verify()
    {
        Regex regex = new(@"^(?!\.)(?!.*\.$)(?!.*__)(?!.*\.\.)[a-zA-Z0-9._]{2,30}$");

        return Username.Equals(Username, StringComparison.OrdinalIgnoreCase) && regex.IsMatch(Username);
    }
}
