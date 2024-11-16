using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.Common;

public abstract class DomainObject
{
    protected static async Task EnsureRuleAsync([NotNull] IAsyncDomainRule rule, CancellationToken ct = default)
    {
        bool isVerified = await rule.VerifyAsync(ct).ConfigureAwait(false);
        if (!isVerified)
        {
            throw new DomainRuleException(rule);
        }
    }

    protected static void EnsureRule([NotNull] ISyncDomainRule rule)
    {
        bool isVerified = rule.Verify();
        if (!isVerified)
        {
            throw new DomainRuleException(rule);
        }
    }
}
