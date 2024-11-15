using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Streams;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Songs;

public sealed class StreamInfoRepository(AppDbContext db)
    : BaseEntityRepository<StreamInfo>(db), IStreamInfoRepository
{
}
