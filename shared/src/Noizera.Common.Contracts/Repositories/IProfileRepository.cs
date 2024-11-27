using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Domain.Profiles;

namespace Noizera.Common.Contracts.Repositories;

public interface IProfileRepository : IRepository<PublicProfile>
{
    Task<PublicProfile> GetAsync(Guid profileId, CancellationToken ct);

    Task<ProfileQueryResult?> GetProfileAsync(string profilePublicId, CancellationToken ct);

    Task<List<ProfileRelationQueryResult>> GetFollowersAsync(string profilePublicId, CancellationToken ct);

    Task<CountQueryResult> GetFollowersCountAsync(string profilePublicId, CancellationToken ct);

    Task<List<ProfileRelationQueryResult>> GetFollowingsAsync(string profilePublicId, CancellationToken ct);

    Task<CountQueryResult> GetFollowingsCountAsync(string profilePublicId, CancellationToken ct);

    Task<List<ArtistQueryResult>> GetArtistsByTextAsync(string text, CancellationToken ct);
}