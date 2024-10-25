using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace Noizera.Shared.Persistence.S3;

public class S3Context(IAmazonS3 s3, IOptions<S3BucketSettings> s3Settings)
{
    private readonly S3BucketSettings settings = s3Settings.Value;

    public async Task<(long ContentLength, string ContentType, string BucketName)> UploadOriginalAudioAsync(string key, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(settings.OriginalAudio, key, file, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(settings.OriginalAudio, key, ct).ConfigureAwait(false);

        return (result.ContentLength, result.ContentType, settings.OriginalAudio);
    }

    public async Task<(long ContentLength, string BucketName)> UploadCoverImageAsync(string key, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(settings.CoverImages, key, file, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(settings.CoverImages, key, ct).ConfigureAwait(false);

        return (result.ContentLength, settings.OriginalAudio);
    }

    public async Task<(long ContentLength, string BucketName)> UploadFlacAudioAsync(string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(settings.FlacAudio, key, contentType, fileStream, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(settings.FlacAudio, key, ct).ConfigureAwait(false);

        return (result.ContentLength, settings.FlacAudio);
    }

    public async Task<(long ContentLength, string BucketName)> UploadMp3AudioAsync(string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        await UploadStreamAsync(settings.Mp3Audio, key, contentType, fileStream, ct).ConfigureAwait(false);

        var result = await GetFileInfoAsync(settings.Mp3Audio, key, ct).ConfigureAwait(false);

        return (result.ContentLength, settings.Mp3Audio);
    }

    public async Task UploadEmailTemplateAsync(string templateName, IFormFile file, CancellationToken ct)
    {
        await UploadFileAsync(settings.EmailTemplates, templateName, file, ct).ConfigureAwait(false);
    }

    public async Task DeleteOriginalAudioAsync(string key, CancellationToken ct)
    {
        await DeleteFileAsync(settings.OriginalAudio, key, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetCoverImageAsync(string key, CancellationToken ct)
    {
        return await GetFileAsync(settings.CoverImages, key, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetOriginalAudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(settings.OriginalAudio, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetFlacAudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(settings.FlacAudio, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)> GetMp3AudioAsync(string key, long start, long end, CancellationToken ct)
    {
        return await GetFilePartAsync(settings.Mp3Audio, key, start, end, ct).ConfigureAwait(false);
    }

    public async Task<GetObjectResponse> GetOriginalAudioFileAsync(string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.OriginalAudio,
            Key = key
        };

        return await s3.GetObjectAsync(request, ct);
    }

    public async Task<string> GetEmailTemplateContentAsync(string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = settings.EmailTemplates,
            Key = key
        };

        string result;
        using (GetObjectResponse response = await s3.GetObjectAsync(request))
        using (StreamReader reader = new StreamReader(response.ResponseStream))
        {
            result = await reader.ReadToEndAsync();
        }

        return result;
    }

    private async Task<(long ContentLength, string ContentType)> GetFileInfoAsync(string bucketName, string key, CancellationToken ct)
    {
        var metadataRequest = new GetObjectMetadataRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var metadataResponse = await s3.GetObjectMetadataAsync(metadataRequest);

        return (metadataResponse.ContentLength, metadataResponse.Headers.ContentType);
    }

    private async Task UploadStreamAsync(string bucketName, string key, string contentType, FileStream fileStream, CancellationToken ct)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentType = contentType,
            InputStream = fileStream
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"File with Id {key} has not been uploaded to bucket {bucketName}.");
        }
    }

    private async Task UploadFileAsync(string bucketName, string key, IFormFile file, CancellationToken ct)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream()
        };

        var result = await s3.PutObjectAsync(request, ct).ConfigureAwait(false);

        if (result.HttpStatusCode is not HttpStatusCode.OK)
        {
            throw new Exception($"File with Id {key} has not been uploaded to bucket {bucketName}.");
        }
    }

    private async Task DeleteFileAsync(string bucketName, string key, CancellationToken ct)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await s3.DeleteObjectAsync(deleteObjectRequest);
    }

    private async Task<(Stream Stream, string ContentType)> GetFileAsync(string bucketName, string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key,
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false)
            ?? throw new Exception($"File with Id {key} has not been found in bucket {bucketName}.");


        return (result.ResponseStream, result.Headers["Content-Type"]);
    }

    private async Task<(Stream Stream, string ContentType)> GetFilePartAsync(string bucketName, string key, long start, long end, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ByteRange = new ByteRange(start, end)
        };

        var result = await s3.GetObjectAsync(request, ct).ConfigureAwait(false) 
            ?? throw new Exception($"File with Id {key} has not been found in bucket {bucketName}.");


        return (result.ResponseStream, result.Headers["Content-Type"]);
    }
}
