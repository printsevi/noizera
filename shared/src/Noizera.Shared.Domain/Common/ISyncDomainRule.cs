namespace Noizera.Shared.Domain.Common;

public interface ISyncDomainRule : IDomainRule
{
    bool Verify();
}
