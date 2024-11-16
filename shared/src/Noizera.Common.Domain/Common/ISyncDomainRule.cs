namespace Noizera.Common.Domain.Common;

public interface ISyncDomainRule : IDomainRule
{
    bool Verify();
}
