using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Streams;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Songs;

public sealed class StreamInfoRepository(AppDbContext db)
    : BaseRepository<StreamInfo>(db), IStreamInfoRepository
{
}
