using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Noizera.Shared.Persistence.S3;

public class S3Context(IAmazonS3 s3, IOptions<S3BucketSettings> s3Settings)
{
    private readonly S3BucketSettings settings = s3Settings.Value;

    private const string OriginalAudioFolder = "original-audio";
    private const string EmailTemplatesFolder = "email-templates";
    private const string CoverImagesFolder = "cover-images";
    private const string FlacAudioFolder = "flac-audio";
    private const string Mp3AudioFolder = "mp3-audio";

    public async Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioAsync(string key, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(OriginalAudioFolder, key, file, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(OriginalAudioFolder, key, ct).ConfigureAwait(false);

        return (result.ContentLength, result.ContentType, OriginalAudioFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadCoverImageAsync(string key, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(CoverImagesFolder, key, file, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(CoverImagesFolder, key, ct).ConfigureAwait(false);

        return (result.ContentLength, CoverImagesFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadFlacAudioAsync(string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(FlacAudioFolder, key, contentType, fileStream, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(FlacAudioFolder, key, ct).ConfigureAwait(false);

        return (result.ContentLength, FlacAudioFolder);
    }

    public async Task<(long ContentLength, string BucketName)> UploadMp3AudioAsync(string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(Mp3AudioFolder, key, contentType, fileStream, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(Mp3AudioFolder, key, ct).ConfigureAwait(false);

        return (result.ContentLength, Mp3AudioFolder);
    }

    public async Task UploadEmailTemplateAsync(string templateName, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(EmailTemplatesFolder, templateName, file, ct).ConfigureAwait(false);
    }

    public async Task DeleteOriginalAudioAsync(string key, CancellationToken ct)
    {
        await DeleteFileAsync(OriginalAudioFolder, key, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetCoverImageAsync(string key, CancellationToken ct)
    {
        return await GetFileAsync(CoverImagesFolder, key, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetOriginalAudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(OriginalAudioFolder, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetFlacAudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(FlacAudioFolder, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetMp3AudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(Mp3AudioFolder, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<GetObjectResponse> GetOriginalAudioFileAsync(string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{OriginalAudioFolder}/{key}"
        };

        return await s3.GetObjectAsync(request, ct);
    }

    public async Task<string> GetEmailTemplateContentAsync(string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{EmailTemplatesFolder}/{key}.html"
        };

        string result;
        using (GetObjectResponse response = await s3.GetObjectAsync(request))
        using (StreamReader reader = new StreamReader(response.ResponseStream))
        {
            result = await reader.ReadToEndAsync();
        }

        return result;
    }

    private async Task<(long ContentLength, string ContentType)> GetFileInfoAsync(string folderName, string key, CancellationToken ct)
    {
        var metadataRequest = new GetObjectMetadataRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
        };

        var metadataResponse = await s3.GetObjectMetadataAsync(metadataRequest);

        return (metadataResponse.ContentLength, metadataResponse.Headers.ContentType);
    }

    private async Task UploadStreamAsync(string folderName, string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        var request = new PutObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
            ContentType = contentType,
            InputStream = fileStream
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"File with Id {key} has not been uploaded to bucket {folderName}.");
        }
    }

    private async Task UploadFileAsync(string folderName, string key, IFormFile file, CancellationToken ct)
    {
        var request = new PutObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream()
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"File with Id {key} has not been uploaded to folder {folderName}.");
        }
    }

    private async Task DeleteFileAsync(string folderName, string key, CancellationToken ct)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
        };

        var response = await s3.DeleteObjectAsync(deleteObjectRequest);
    }

    private async Task<(Stream Stream, string ContentType)> GetFileAsync(string folderName, string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false)
            ?? throw new Exception($"File with Id {key} has not been found in folder {folderName}.");


        return (result.ResponseStream, result.Headers["Content-Type"]);
    }

    private async Task<(Stream Stream, string ContentType)> GetFilePartAsync(string folderName, string key, long start, long end, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.BucketName,
            Key = $"{folderName}/{key}",
            ByteRange = new ByteRange(start, end)
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false)
            ?? throw new Exception($"File with Id {key} has not been found in folder {folderName}.");


        return (result.ResponseStream, result.Headers["Content-Type"]);
    }
}
