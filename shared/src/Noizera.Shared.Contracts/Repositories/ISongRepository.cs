using Noizera.Shared.Domain.Songs;

namespace Noizera.Shared.Contracts.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<Song?> GetAsync(Guid id, CancellationToken ct);

    Task<Song?> GetAsync(string publicId, CancellationToken ct);

    Task DeleteAsync(Guid songId, CancellationToken ct);
}
