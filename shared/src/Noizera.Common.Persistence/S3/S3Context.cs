using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Noizera.Common.Persistence.S3;

public class S3Context(IAmazonS3 s3, IOptions<S3BucketSettings> s3Settings)
{
    private readonly S3BucketSettings settings = s3Settings.Value;

    private const string originalAudioFolder = "original-audio";
    private const string coverImagesFolder = "cover-images";
    private const string profileImagesFolder = "profile-images";
    private const string flacAudioFolder = "flac-audio";
    private const string mp3AudioFolder = "mp3-audio";

    public async Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioAsync([NotNull] string key, [NotNull] IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(originalAudioFolder, key, file, ct).ConfigureAwait(false);

        var (ContentLength, ContentType) = await GetFileInfoAsync(originalAudioFolder, key, ct).ConfigureAwait(false);

        return (ContentLength, ContentType, originalAudioFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadCoverImageAsync([NotNull] string key, [NotNull] IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(coverImagesFolder, key, file, ct).ConfigureAwait(false);

        var (ContentLength, _) = await GetFileInfoAsync(coverImagesFolder, key, ct).ConfigureAwait(false);

        return (ContentLength, coverImagesFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadProfileImageAsync([NotNull] string key, [NotNull] IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(profileImagesFolder, key, file, ct).ConfigureAwait(false);

        var (ContentLength, _) = await GetFileInfoAsync(profileImagesFolder, key, ct).ConfigureAwait(false);

        return (ContentLength, coverImagesFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadFlacAudioAsync([NotNull] string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(flacAudioFolder, key, contentType, fileStream, ct).ConfigureAwait(false);

        var (ContentLength, _) = await GetFileInfoAsync(flacAudioFolder, key, ct).ConfigureAwait(false);

        return (ContentLength, flacAudioFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadMp3AudioAsync([NotNull] string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(mp3AudioFolder, key, contentType, fileStream, ct).ConfigureAwait(false);

        var (ContentLength, _) = await GetFileInfoAsync(mp3AudioFolder, key, ct).ConfigureAwait(false);
        return (ContentLength, mp3AudioFolder);
    }

    public async Task DeleteProfileImageAsync([NotNull] string key, CancellationToken ct)
        => await DeleteFileAsync(profileImagesFolder, key, ct).ConfigureAwait(false);

    public async Task DeleteOriginalAudioAsync([NotNull] string key, CancellationToken ct)
        => await DeleteFileAsync(originalAudioFolder, key, ct).ConfigureAwait(false);

    public async Task<(Stream Stream, string ContentType)> GetCoverImageAsync([NotNull] string key, CancellationToken ct)
        => await GetFileAsync(coverImagesFolder, key, ct).ConfigureAwait(false)
            ?? throw new AmazonS3Exception($"File with Id {key} has not been found in folder {coverImagesFolder}.");

    public async Task<(Stream Stream, string ContentType)?> GetProfileImageAsync([NotNull] string key, CancellationToken ct)
        => await GetFileAsync(profileImagesFolder, key, ct).ConfigureAwait(false);

    public async Task<(Stream Stream, string ContentType)> GetOriginalAudioAsync([NotNull] string key, long start, long end, CancellationToken ct)
        => await GetFilePartAsync(originalAudioFolder, key, start, end, ct).ConfigureAwait(false);

    public async Task<(Stream Stream, string ContentType)> GetFlacAudioAsync([NotNull] string key, long start, long end, CancellationToken ct)
        => await GetFilePartAsync(flacAudioFolder, key, start, end, ct).ConfigureAwait(false);

    public async Task<(Stream Stream, string ContentType)> GetMp3AudioAsync([NotNull] string key, long start, long end, CancellationToken ct)
        => await GetFilePartAsync(mp3AudioFolder, key, start, end, ct).ConfigureAwait(false);

    public async Task<GetObjectResponse> GetOriginalAudioFileAsync([NotNull] string key, CancellationToken ct)
    {
        GetObjectRequest request = new()
        {
            BucketName = settings.BucketName,
            Key = $"{originalAudioFolder}/{key.ToUpperInvariant()}"
        };

        return await s3.GetObjectAsync(request, ct).ConfigureAwait(false);
    }

    private async Task<(long ContentLength, string ContentType)> GetFileInfoAsync(string folderName, [NotNull] string key, CancellationToken ct)
    {
        GetObjectMetadataRequest metadataRequest = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
        };

        var metadataResponse = await s3.GetObjectMetadataAsync(metadataRequest, ct).ConfigureAwait(false);

        return (metadataResponse.ContentLength, metadataResponse.Headers.ContentType);
    }

    private async Task UploadStreamAsync(string folderName, [NotNull] string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        PutObjectRequest request = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
            ContentType = contentType,
            InputStream = fileStream
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new AmazonS3Exception($"File with Id {key} has not been uploaded to bucket {folderName}.");
        }
    }

    private async Task UploadFileAsync(string folderName, [NotNull] string key, IFormFile file, CancellationToken ct)
    {
        PutObjectRequest request = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream()
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new AmazonS3Exception($"File with Id {key} has not been uploaded to folder {folderName}.");
        }
    }

    private async Task DeleteFileAsync(string folderName, [NotNull] string key, CancellationToken ct)
    {
        DeleteObjectRequest deleteObjectRequest = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
        };

        _ = await s3.DeleteObjectAsync(deleteObjectRequest, ct).ConfigureAwait(false);
    }

    private async Task<(Stream Stream, string ContentType)?> GetFileAsync(string folderName, [NotNull] string key, CancellationToken ct)
    {
        GetObjectRequest request = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false);
        return result is null ? null : (result.ResponseStream, result.Headers["Content-Type"]);
    }

    private async Task<(Stream Stream, string ContentType)> GetFilePartAsync(string folderName, [NotNull] string key, long start, long end, CancellationToken ct)
    {
        GetObjectRequest request = new()
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key.ToUpperInvariant()}",
            ByteRange = new ByteRange(start, end)
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false)
            ?? throw new AmazonS3Exception($"File with Id {key} has not been found in folder {folderName}.");

        return (result.ResponseStream, result.Headers["Content-Type"]);
    }
}
