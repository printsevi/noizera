using HashidsNet;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Common;

public abstract class BaseRepository<TEntity>(AppDbContext db)
    : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected AppDbContext Db => db;

    public async Task InsertAsync(TEntity entity, CancellationToken ct)
    {
        if (entity is EntityExtended entityExtended && string.IsNullOrWhiteSpace(entityExtended.PublicId))
        {
            Hashids hashids = new("CDB581A849B94AC899BB0ACDE57286B9");
            long numberId = await db.GetNextNumberIdSequenceValueAsync(ct).ConfigureAwait(false);
            string hash = hashids.EncodeLong(numberId).ToLower();

            entityExtended.SetPublicId(hash);

            _ = await db.AddAsync(entityExtended, ct).ConfigureAwait(false);
        }
        else
        {
            _ = await db.AddAsync(entity, ct).ConfigureAwait(false);
        }
        _ = await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct)
    {
        if (entity is Entity entityWithDates)
        {
            //entityWithDates.SetLastModifiedOnAsNow();
        }

        _ = db.Update(entity);
        _ = await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
