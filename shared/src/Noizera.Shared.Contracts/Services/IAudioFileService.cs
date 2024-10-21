using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Shared.Contracts.Services;

public interface IAudioFileService
{
    Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioFileAsync(IFormFile file, string fileId, CancellationToken ct);

    Task DeleteOriginalAudioFileAsync(string fileId, CancellationToken ct);

    Task<AudioStreamResult> GetAudioFileAsStream(string fileId, string requestedRange, long fileLength, string audioType, CancellationToken ct);
}
