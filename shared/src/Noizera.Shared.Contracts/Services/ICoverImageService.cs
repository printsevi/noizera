using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Contracts.Services;

public interface ICoverImageService
{
    Task<(long ContentLength, string BucketName)> UploadImageAsync([NotNull] IFormFile file, string fileId, CancellationToken ct);

    Task<(Stream Stream, string ContentType)> GetImageFileAsStreamAsync(string fileId, CancellationToken ct);
}
