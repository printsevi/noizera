using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.Profiles;

public interface IProfileImageUploader
{
    Task<(long ContentLength, string BucketName)> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct);
}
