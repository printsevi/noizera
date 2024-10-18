using MediatR;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetArtists;

public sealed record GetArtistsQuery(string Text, Guid UserId) : IAuthorizeableRequest<GetArtistsResponse>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetArtistsQuery, GetArtistsResponse>
    {
        public async Task<GetArtistsResponse> Handle([NotNull] GetArtistsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return new([]);
            }

            var result = await profileRepository.GetArtistsByTextAsync(request.Text, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
