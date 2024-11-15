using Noizera.Common.Domain.ListeningHistories;

namespace Noizera.Common.Contracts.Repositories;

public interface IListeningHistoryRepository : IRepository<ListeningHistory>
{
    Task<ListeningHistory?> GetAsync(Guid userId, Guid songId);
}
