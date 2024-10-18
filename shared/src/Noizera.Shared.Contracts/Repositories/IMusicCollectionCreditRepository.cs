using Noizera.Shared.Domain.MusicCollectionCredits;

namespace Noizera.Shared.Contracts.Repositories;

public interface IMusicCollectionCreditRepository : IRepository<MusicCollectionCredit>
{
    Task DeleteCreditAsync(Guid creditId, CancellationToken ct);
}
