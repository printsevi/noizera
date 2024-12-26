using Microsoft.AspNetCore.Http;
using Noizera.Common.Contracts.QueryResults;

namespace Noizera.Common.Contracts.Services;

public interface IAudioFileService
{
    Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioFileAsync(IFormFile file, string fileId, CancellationToken ct);

    Task DeleteOriginalAudioFileAsync(string fileId, CancellationToken ct);

    Task<AudioStreamResult> GetAudioFileAsStream(string fileId, string requestedRange, long fileLength, string audioType, CancellationToken ct);

    Task<Uri> GetAudioPresignedUrlAsync(string fileId, string audioType, CancellationToken ct);
}
