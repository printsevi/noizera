using Microsoft.AspNetCore.Http;
using Noizera.Common.Contracts.Services;
using Noizera.Common.Domain.MusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.CoverImages;

public class CoverImageUploader(ICoverImageService service) : ICoverImageUploader
{
    public async Task<(long ContentLength, string BucketName)> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
        => await service.UploadImageAsync(file, fileId, ct).ConfigureAwait(false);
}
