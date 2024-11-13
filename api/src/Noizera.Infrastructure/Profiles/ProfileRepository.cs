using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using Noizera.Infrastructure.Common;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.ProfileRelations;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.Infrastructure.Profiles;

public sealed class ProfileRepository(AppDbContext db, IHashGenerator hashGenerator)
    : BaseEntityExtendedRepository<PublicProfile>(db, hashGenerator), IProfileRepository
{
    public async Task<List<ArtistQueryResult>> GetArtistsByTextAsync(string text, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                p."Id" as ArtistId,
                p."Name" as Name,
                p."PublicId" as PublicId
                SIMILARITY(p."Name", {text}) AS Score
            FROM public."Profiles" p
            WHERE
                p."ProfileType" = 'Artist'
                AND SIMILARITY(p."Name", {text}) > 0.2
            ORDER BY Name, Score DESC
            LIMIT 10;
            """;

        var result = await Db.Database.SqlQuery<ArtistQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<List<ArtistQueryResult>> SearchAsync(string searchQuery, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                p."Id" as ArtistId,
                p."Name" as Name
            FROM public."Profiles" p
            WHERE
                p."ProfileType" = 'Artist'
                AND SIMILARITY(p."Name", {searchQuery}) > 0.4;
            """;

        var result = await Db.Database.SqlQuery<ArtistQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<List<FastSearchQueryResult>> FastSearchAsync(string searchQuery, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return [];
        }

        FormattableString sql = $"""
            WITH Results AS (
                SELECT 
                    p."Name" as Value, 
                    SIMILARITY(p."Name", {searchQuery}) AS Score
                FROM 
                    public."Profiles" p
                WHERE 
                    SIMILARITY(p."Name", {searchQuery}) > 0.1

                UNION ALL

                SELECT 
                    s."Title" as Value, 
                    SIMILARITY(s."Title", {searchQuery}) AS Score
                FROM 
                    public."Songs" s
                WHERE 
                    SIMILARITY(s."Title", {searchQuery}) > 0.1

                UNION ALL

                SELECT 
                    mc."Title" as Value, 
                    SIMILARITY(mc."Title", {searchQuery}) AS Score
                FROM 
                    public."MusicSets" mc
                WHERE 
                    SIMILARITY(mc."Title", {searchQuery}) > 0.1
            )
            SELECT DISTINCT ON (Value) *
            FROM Results
            ORDER BY Value, Score DESC
            LIMIT 10;
            """;

        var result = await Db.Database.SqlQuery<FastSearchQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public Task<PublicProfile> GetAsync(Guid profileId, CancellationToken ct) => throw new NotImplementedException();

    public async Task<ProfileQueryResult?> GetProfileAsync(string profilePublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT 
                p."Name" as Name,
                p."Bio" as Bio,
                (
                    SELECT
                        COUNT(*)
                    FROM 
                        public."ProfileRelations" pr
                    JOIN 
                        public."Profiles" p2
                            ON p2."Id" = pr."FollowingProfileId"
                    WHERE
                        pr."FollowerProfileId" = p."Id"
                        AND p2."IsDeleted" = false
                 ) as FollowersCount,
                (
                    SELECT
                        COUNT(*)
                    FROM 
                        public."ProfileRelations" pr
                    JOIN 
                        public."Profiles" p2
                            ON p2."Id" = pr."FollowerProfileId"
                    WHERE
                        pr."FollowingProfileId" = p."Id"
                        AND p2."IsDeleted" = false
                 ) as FollowingsCount
            FROM 
                public."Profiles" p
            WHERE 
                p."PublicId" = {profilePublicId} 
                AND p."IsDeleted" = false
            LIMIT 1;
            """;

        var result = await Db.Database
            .SqlQuery<ProfileQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    public async Task<List<ProfileRelationQueryResult>> GetFollowersAsync(string profilePublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                p."{nameof(PublicProfile.PublicId)}" as {nameof(ProfileRelationQueryResult.ProfilePublicId)},
                p."{nameof(PublicProfile.Name)}" as {nameof(ProfileRelationQueryResult.Name)}
            FROM 
                public."{nameof(Db.Profiles)}" p
            JOIN 
                public."{nameof(Db.ProfileRelations)}" pr 
                    ON p."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowerProfileId)}"
            JOIN 
                public."{nameof(Db.Profiles)}" p2 
                    ON p2."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowingProfileId)}"
            WHERE
                p2."{nameof(PublicProfile.PublicId)}" = '{profilePublicId}'
                AND p2."{nameof(PublicProfile.IsDeleted)}" = false;
            """;

        var result = await Db.Database.SqlQuery<ProfileRelationQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<CountQueryResult> GetFollowersCountAsync(string profilePublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                COUNT(p."Id") as Count
            FROM 
                public."Profiles" p
            JOIN 
                public."ProfileRelations" pr 
                    ON p."Id" = pr."FollowerProfileId"
            JOIN 
                public."Profiles" p2 
                    ON p2."Id" = pr."FollowingProfileId"
            WHERE
                p2."PublicId" = {profilePublicId}
                AND p2."IsDeleted" = false;
            """;

        var result = await Db.Database.SqlQuery<CountQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result.FirstOrDefault() ?? new(0);
    }

    public async Task<List<ProfileRelationQueryResult>> GetFollowingsAsync(string profilePublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                p."{nameof(PublicProfile.PublicId)}" as {nameof(ProfileRelationQueryResult.ProfilePublicId)},
                p."{nameof(PublicProfile.Name)}" as {nameof(ProfileRelationQueryResult.Name)}
            FROM 
                public."{nameof(Db.Profiles)}" p
            JOIN 
                public."{nameof(Db.ProfileRelations)}" pr 
                    ON p."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowingProfileId)}"
            JOIN 
                public."{nameof(Db.Profiles)}" p2 
                    ON p2."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowerProfileId)}"
            WHERE
                p2."{nameof(PublicProfile.PublicId)}" = '{profilePublicId}'
                AND p2."{nameof(PublicProfile.IsDeleted)}" = false;
            """;

        var result = await Db.Database.SqlQuery<ProfileRelationQueryResult>(sql).ToListAsync(ct).ConfigureAwait(false);

        return result;
    }

    public async Task<CountQueryResult> GetFollowingsCountAsync(string profilePublicId, CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT
                COUNT(p."{nameof(PublicProfile.Id)}") as {nameof(CountQueryResult.Count)}
            FROM 
                public."{nameof(Db.Profiles)}" p
            JOIN 
                public."{nameof(Db.ProfileRelations)}" pr 
                    ON p."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowingProfileId)}"
            JOIN 
                public."{nameof(Db.Profiles)}" p2 
                    ON p2."{nameof(PublicProfile.Id)}" = pr."{nameof(ProfileRelation.FollowerProfileId)}"
            WHERE
                p2."{nameof(PublicProfile.PublicId)}" = '{profilePublicId}'
                AND p2."{nameof(PublicProfile.IsDeleted)}" = false;;
            """;

        var result = await Db.Database.SqlQuery<CountQueryResult>(sql).FirstOrDefaultAsync(ct).ConfigureAwait(false);

        return result ?? new(0);
    }
}
