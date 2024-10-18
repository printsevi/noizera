using Noizera.Shared.Domain.Terms;

namespace Noizera.Shared.Contracts.Repositories;

public interface ITermsRepository : IRepository<TermsOfUse>
{
    Task<TermsOfUse?> GetLatestAsync(CancellationToken ct);
}
