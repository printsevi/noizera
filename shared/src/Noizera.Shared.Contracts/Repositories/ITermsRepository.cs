using Noizera.Common.Domain.Terms;

namespace Noizera.Common.Contracts.Repositories;

public interface ITermsRepository : IRepository<TermsOfUse>
{
    Task<TermsOfUse?> GetLatestAsync(CancellationToken ct);
}
