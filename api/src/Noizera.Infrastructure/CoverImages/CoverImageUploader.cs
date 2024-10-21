using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.MusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.CoverImages;

public class CoverImageUploader(ICoverImageService service) : ICoverImageUploader
{
    public async Task<(long ContentLength, string BucketName)> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
    {
        return await service.UploadImageAsync(file, fileId, ct);
    }
}
