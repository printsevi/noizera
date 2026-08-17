using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Noizera.Application.Common;
using Noizera.Application.CQRS.Songs.Common;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;

namespace Noizera.Application.CQRS.Songs;

public sealed record GetAudioPresignedUrlQuery(
    string SongPublicId,
    string AudioType,
    Guid UserId)
    : IAuthorizeableRequest<UrlResponse>
{
    public sealed class Handler(
        IAudioFileService audioFileService,
        AudioAccessGuard audioAccessGuard)
        : IRequestHandler<GetAudioPresignedUrlQuery, UrlResponse>
    {
        public async Task<UrlResponse> Handle([NotNull] GetAudioPresignedUrlQuery request, CancellationToken cancellationToken)
        {
            // A presigned URL bypasses the application entirely once issued, so entitlement has
            // to be settled before one is minted.
            _ = await audioAccessGuard.AuthorizeAsync(request.SongPublicId, request.AudioType, request.UserId, cancellationToken).ConfigureAwait(false);

            var result = await audioFileService.GetAudioPresignedUrlAsync(request.SongPublicId, request.AudioType, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}

public sealed class GetAudioPresignedUrlValidator : AbstractValidator<GetAudioPresignedUrlQuery>
{
    public GetAudioPresignedUrlValidator()
    {
        _ = RuleFor(x => x.SongPublicId).NotEmpty();
        _ = RuleFor(x => x.AudioType).NotEmpty();
    }
}
