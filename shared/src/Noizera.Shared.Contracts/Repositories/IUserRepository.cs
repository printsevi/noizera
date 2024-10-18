using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Contracts.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername, CancellationToken ct);

    Task<User?> GetAsync(Guid userId, CancellationToken ct);

    Task<User?> GetWithSavedCollectionsAsync(Guid userId, CancellationToken ct);

    Task<User?> GetWithSongsAsync(Guid userId, CancellationToken ct);

    Task<User?> GetByEmailWithLatestResetTokenAsync(string email, CancellationToken ct);

    Task<User?> GetWithActiveRefreshTokensAsync(Guid userId, CancellationToken ct);

    Task<User?> GetWithSubscriptionsAsync(Guid userId, CancellationToken ct);
}
