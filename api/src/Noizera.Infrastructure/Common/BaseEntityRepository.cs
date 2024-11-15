using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Common;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Common;

public abstract class BaseEntityRepository<TEntity>(AppDbContext db)
    : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected AppDbContext Db => db;

    public virtual async Task InsertAsync(TEntity entity, CancellationToken ct)
    {
        _ = await Db.AddAsync(entity, ct).ConfigureAwait(false);
        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        if (entity is Entity entityWithDates)
        {
            //entityWithDates.SetLastModifiedOnAsNow();
        }

        _ = Db.Update(entity);
        _ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
