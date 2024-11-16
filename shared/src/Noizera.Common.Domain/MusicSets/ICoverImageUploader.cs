using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.MusicSets;

public interface ICoverImageUploader
{
    Task<(long ContentLength, string BucketName)> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct);
}
