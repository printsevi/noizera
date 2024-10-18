using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.SecretTokens;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Security.Tokens;

public sealed class SecretTokenRepository(AppDbContext db) : BaseRepository<SecretToken>(db), ISecretTokenRepository
{
}
