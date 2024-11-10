using MediatR;
using Noizera.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.GetCoverImage;

public sealed record GetCoverImageQuery(
    string MusicSetPublicId) : IRequest<(Stream Stream, string ContentType)>
{
    public sealed class Handler(
        ICoverImageService coverImageService)
        : IRequestHandler<GetCoverImageQuery, (Stream Stream, string ContentType)>
    {
        public async Task<(Stream Stream, string ContentType)> Handle([NotNull] GetCoverImageQuery request, CancellationToken cancellationToken)
            => await coverImageService.GetImageFileAsStreamAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false);
    }
}
