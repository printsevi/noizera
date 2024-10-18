using MediatR;
using Noizera.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetCoverImage;

public sealed record GetCoverImageQuery(
    string MusicCollectionPublicId) : IRequest<Stream>
{
    public sealed class Handler(
        ICoverImageService coverImageService)
        : IRequestHandler<GetCoverImageQuery, Stream>
    {
        public async Task<Stream> Handle([NotNull] GetCoverImageQuery request, CancellationToken cancellationToken)
            => await coverImageService.GetImageFileAsStreamAsync(request.MusicCollectionPublicId, cancellationToken).ConfigureAwait(false);
    }
}
