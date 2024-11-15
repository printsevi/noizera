using Noizera.Common.Domain.Songs;

namespace Noizera.Common.Contracts.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<Song?> GetAsync(Guid id, CancellationToken ct);

    Task<Song?> GetAsync(string publicId, CancellationToken ct);

    Task DeleteAsync(Guid songId, CancellationToken ct);
}
