using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetProfile;

public sealed record GetProfileQuery(string Username)
    : IRequest<ProfileQueryResult>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetProfileQuery, ProfileQueryResult>
    {
        public async Task<ProfileQueryResult> Handle([NotNull] GetProfileQuery request, CancellationToken cancellationToken)
        {
            FormattableString sql = $"""
                SELECT 
                    p."Name" as Name,
                    p."PublicId" as PublicId,
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
                    p."Username" = UPPER({request.Username})
                    AND p."IsDeleted" = false
                LIMIT 1;
            """;

            var result = await db.Database
                .SqlQuery<ProfileQueryResult>(sql).ToListAsync(cancellationToken).ConfigureAwait(false);

            return result.FirstOrDefault()
                ?? throw new AppException($"Profile not found for {request.Username}", ErrorType.NotFound);
        }
    }
}
