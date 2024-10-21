using MediatR;
using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.UploadAudioFile;

public sealed record UploadAudioFileCommand(
    IFormFile File,
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<UploadAudioFileResponse>
{
    public sealed class Handler(
        IAudioFileService audioService,
        ISongRepository songRepository)
        : IRequestHandler<UploadAudioFileCommand, UploadAudioFileResponse>
    {
        public async Task<UploadAudioFileResponse> Handle([NotNull] UploadAudioFileCommand request, CancellationToken cancellationToken)
        {
            var song = await songRepository.GetAsync(request.SongId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Song {request.SongId} is not found", ErrorType.NotFound);

            song.ValidateOwner(request.UserId);

            (var fileLength, var contentType, var bucket) = await audioService.UploadOriginalAudioFileAsync(request.File, song.PublicId, cancellationToken).ConfigureAwait(false);

            var originalFileName = ValidFileName.New(request.File.FileName);
            song.UploadOriginalAudioFile(originalFileName, Path.GetExtension(request.File.FileName), fileLength, bucket);

            await songRepository.UpdateAsync(song, cancellationToken).ConfigureAwait(false);

            return new(originalFileName.Value, contentType, fileLength);
        }
    }
}
