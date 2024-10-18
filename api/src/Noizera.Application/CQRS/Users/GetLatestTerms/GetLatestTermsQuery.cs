using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.GetLatestTerms;

public sealed record GetLatestTermsQuery()
    : IRequest<GetLatestTermsResponse>
{
    public sealed class Handler(
        ITermsRepository termsRepository)
        : IRequestHandler<GetLatestTermsQuery, GetLatestTermsResponse>
    {
        public async Task<GetLatestTermsResponse> Handle([NotNull] GetLatestTermsQuery request, CancellationToken cancellationToken)
        {
            var result = await termsRepository.GetLatestAsync(cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Terms are not found", ErrorType.NotFound);

            return new(result.Content, result.EffectiveDate);
        }
    }
}
