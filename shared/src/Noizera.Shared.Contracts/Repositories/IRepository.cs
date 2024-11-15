using Noizera.Common.Domain.Common;

namespace Noizera.Common.Contracts.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity, CancellationToken ct);

    Task UpdateAsync(TEntity entity, CancellationToken ct);
}
