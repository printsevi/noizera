using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.DeleteAudioFile;

public sealed record DeleteAudioFileCommand(
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAudioFileService audioService,
        ISongRepository songRepository)
        : IRequestHandler<DeleteAudioFileCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteAudioFileCommand request, CancellationToken cancellationToken)
        {
            var song = await songRepository.GetAsync(request.SongId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Song {request.SongId} is not found", ErrorType.NotFound);

            song.ValidateOwner(request.UserId);

            audioService.DeleteOriginalAudioFile(song.PublicId);

            song.DeleteOriginalAudioFile();

            await songRepository.UpdateAsync(song, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
