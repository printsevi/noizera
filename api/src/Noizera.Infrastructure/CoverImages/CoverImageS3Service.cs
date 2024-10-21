using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Persistence.S3;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.CoverImages;

public class CoverImageS3Service(S3Context s3) : ICoverImageService
{
    public async Task<(long ContentLength, string BucketName)> UploadImageAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
    {
        return await s3.UploadCoverImageAsync(fileId, file, ct);
    }

    public async Task<(Stream Stream, string ContentType)> GetImageFileAsStreamAsync(string fileId, CancellationToken ct)
    {
        return await s3.GetCoverImageAsync(fileId, ct).ConfigureAwait(false);
    }
}
