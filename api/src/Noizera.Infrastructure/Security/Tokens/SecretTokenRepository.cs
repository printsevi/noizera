using Noizera.Infrastructure.Common;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.SecretTokens;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Security.Tokens;

public sealed class SecretTokenRepository(AppDbContext db) : BaseEntityRepository<SecretToken>(db), ISecretTokenRepository
{
}
