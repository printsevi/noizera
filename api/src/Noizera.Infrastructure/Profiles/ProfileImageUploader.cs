using Microsoft.AspNetCore.Http;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Persistence.S3;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.Profiles;

public class ProfileImageUploader(S3Context s3Context) : IProfileImageUploader
{
    public async Task<(long ContentLength, string BucketName)> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
        => await s3Context.UploadProfileImageAsync(fileId, file, ct).ConfigureAwait(false);
}
