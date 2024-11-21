using Noizera.Common.Domain.Common;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Common;

public abstract class BaseEntityExtendedRepository<TEntityExtended>(
    AppDbContext dbContext)
    : BaseEntityRepository<TEntityExtended>(dbContext)
    where TEntityExtended : EntityExtended
{
    public override async Task InsertAsync(TEntityExtended entity, CancellationToken ct)
    {
        //var hash = await hashGenerator.GenerateAsync(ct).ConfigureAwait(false);

        //entity.SetPublicId(hash);

        _ = await Db.AddAsync(entity, ct).ConfigureAwait(false);
        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
