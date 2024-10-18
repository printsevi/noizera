using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Contracts.Repositories;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity, CancellationToken ct);

    Task UpdateAsync(TEntity entity, CancellationToken ct);
}
