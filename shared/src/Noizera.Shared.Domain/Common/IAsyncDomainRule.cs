namespace Noizera.Common.Domain.Common;

public interface IAsyncDomainRule : IDomainRule
{
    Task<bool> VerifyAsync(CancellationToken ct = default);
}
