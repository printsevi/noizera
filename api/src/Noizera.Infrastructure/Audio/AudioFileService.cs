using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Services;
using Noizera.Common.Infrastructure.Audio;
using Noizera.Common.Persistence.S3;

namespace Noizera.Infrastructure.Audio;

public class AudioFileService(
    IOptions<AudioSettings> audioSettings,
    S3Context s3)
    : IAudioFileService
{
    private readonly AudioSettings settings = audioSettings.Value;

    public async Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioFileAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
    {
        string extension = Path.GetExtension(file.FileName);

        return !settings.ValidInputExtensions.Contains(extension)
            ? throw new AppException($"Invalid file's extension {extension}", ErrorType.Validation)
            : await s3.UploadOriginalAudioAsync(fileId, file, ct).ConfigureAwait(false);
    }

    public async Task DeleteOriginalAudioFileAsync(string fileId, CancellationToken ct)
        => await s3.DeleteOriginalAudioAsync(fileId, ct).ConfigureAwait(false);

    public async Task<AudioStreamResult> GetAudioFileAsStream(string fileId, string requestedRange, long fileLength, string audioType, CancellationToken ct) => audioType switch
    {
        "original" => await GetOriginalAudioFileAsStream(fileId, requestedRange, fileLength, ct).ConfigureAwait(false),
        "audio/flac" => await GetFlacAudioFileAsStream(fileId, requestedRange, fileLength, ct).ConfigureAwait(false),
        "audio/mpeg" => await GetMp3AudioFileAsStream(fileId, requestedRange, fileLength, ct).ConfigureAwait(false),
        _ => throw new ArgumentException($"Content type is undefined {audioType}")
    };

    public async Task<Uri> GetAudioPresignedUrlAsync(string fileId, string audioType, CancellationToken ct) => audioType switch
    {
        "audio/flac" => await s3.GetFlacAudioPresignedLinkAsync(fileId, ct).ConfigureAwait(false),
        "audio/mpeg" => await s3.GetMp3AudioPresignedLinkAsync(fileId, ct).ConfigureAwait(false),
        _ => throw new ArgumentException($"Content type is undefined {audioType}")
    };

    private async Task<AudioStreamResult> GetOriginalAudioFileAsStream(string fileId, string requestedRange, long fileLength, CancellationToken ct)
    {
        const long FILE_PORTION_SIZE = 2000000; // 2MB

        long start = 0L;
        if (!string.IsNullOrEmpty(requestedRange))
        {
            string[] range = requestedRange.Replace("bytes=", "", StringComparison.OrdinalIgnoreCase).Split('-');
            start = long.Parse(range[0], CultureInfo.InvariantCulture);
        }

        long end = Math.Min(start + FILE_PORTION_SIZE, fileLength - 1);
        long partLength = end - start + 1;

        var result = await s3.GetOriginalAudioAsync(fileId, start, end, ct).ConfigureAwait(false);

        return new(result.Stream, fileLength, result.ContentType, partLength, start, end);
    }

    private async Task<AudioStreamResult> GetFlacAudioFileAsStream(string fileId, string requestedRange, long fileLength, CancellationToken ct)
    {
        const long FILE_PORTION_SIZE = 500000; // 0.5MB

        long start = 0L;
        if (!string.IsNullOrEmpty(requestedRange))
        {
            string[] range = requestedRange.Replace("bytes=", "", StringComparison.OrdinalIgnoreCase).Split('-');
            start = long.Parse(range[0], CultureInfo.InvariantCulture);
        }

        long end = Math.Min(start + FILE_PORTION_SIZE, fileLength - 1);
        long partLength = end - start + 1;

        var result = await s3.GetFlacAudioAsync(fileId, start, end, ct).ConfigureAwait(false);

        return new(result.Stream, fileLength, result.ContentType, partLength, start, end);
    }

    private async Task<AudioStreamResult> GetMp3AudioFileAsStream(string fileId, string requestedRange, long fileLength, CancellationToken ct)
    {
        const long FILE_PORTION_SIZE = 500000; // 0.5MB

        long start = 0L;
        if (!string.IsNullOrEmpty(requestedRange))
        {
            string[] range = requestedRange.Replace("bytes=", "", StringComparison.OrdinalIgnoreCase).Split('-');
            start = long.Parse(range[0], CultureInfo.InvariantCulture);
        }

        long end = Math.Min(start + FILE_PORTION_SIZE, fileLength - 1);
        long partLength = end - start + 1;

        var result = await s3.GetMp3AudioAsync(fileId, start, end, ct).ConfigureAwait(false);

        return new(result.Stream, fileLength, result.ContentType, partLength, start, end);
    }
}
