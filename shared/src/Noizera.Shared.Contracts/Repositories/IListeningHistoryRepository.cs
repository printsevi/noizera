using Noizera.Shared.Domain.ListeningHistories;

namespace Noizera.Shared.Contracts.Repositories;

public interface IListeningHistoryRepository : IRepository<ListeningHistory>
{
    Task<ListeningHistory?> GetAsync(Guid userId, Guid songId);
}
