using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Infrastructure.Audio;
using Noizera.Shared.Infrastructure.DataStructure;
using Noizera.Shared.Persistence.S3;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.Audio;

public class AudioFileService(
    IOptions<AudioSettings> audioSettings,
    S3Context s3) 
    : IAudioFileService
{
    private readonly AudioSettings settings = audioSettings.Value;

    public async Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioFileAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
    {
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!settings.ValidInputExtensions.Contains(extension))
        {
            throw new AppException($"Invalid file's extension {extension}", ErrorType.Validation);
        }

        return await s3.UploadOriginalAudioAsync(fileId, file, ct).ConfigureAwait(false);
    }

    public async Task DeleteOriginalAudioFileAsync(string fileId, CancellationToken ct)
    {
        await s3.DeleteOriginalAudioAsync(fileId, ct).ConfigureAwait(false);
    }

    public async Task<AudioStreamResult> GetAudioFileAsStream(string fileId, string requestedRange, long fileLength, string audioType, CancellationToken ct)
    {
        if (audioType == "original")
        {
            return await GetOriginalAudioFileAsStream(fileId, requestedRange, fileLength, ct);
        }
        else
        {
            throw new Exception();
        }
    }

    private async Task<AudioStreamResult> GetOriginalAudioFileAsStream(string fileId, string requestedRange, long fileLength, CancellationToken ct)
    {
        const long FILE_PORTION_SIZE = 2000000; // 2MB

        var start = 0L;
        if (!string.IsNullOrEmpty(requestedRange))
        {
            var range = requestedRange.Replace("bytes=", "").Split('-');
            start = long.Parse(range[0]);
        }

        var end = Math.Min(start + FILE_PORTION_SIZE, fileLength - 1);
        var partLength = end - start + 1;

        var result = await s3.GetOriginalAudioAsync(fileId, start, end, ct);

        return new(result.Stream, fileLength, result.ContentType, partLength, start, end);
    }
}
