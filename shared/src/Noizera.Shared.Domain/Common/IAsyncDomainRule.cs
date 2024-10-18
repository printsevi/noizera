namespace Noizera.Shared.Domain.Common;

public interface IAsyncDomainRule : IDomainRule
{
    Task<bool> VerifyAsync(CancellationToken ct = default);
}
