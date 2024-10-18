using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.Common;

public class DomainRuleException : Exception
{
    public IDomainRule BrokenRule { get; } = new DefaultRule();

    public DomainRuleException([NotNull] IDomainRule brokenRule)
        : base(brokenRule.ErrorMessage) => BrokenRule = brokenRule;

    public override string ToString() => $"{BrokenRule.GetType().FullName}: {BrokenRule.ErrorMessage}";

    public DomainRuleException()
    {
    }

    public DomainRuleException(string message) : base(message)
    {
    }

    public DomainRuleException(string message, Exception innerException) : base(message, innerException)
    {
    }

    private sealed class DefaultRule : IDomainRule
    {
        public string ErrorMessage => "Default Rule";
    }
}
