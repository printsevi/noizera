using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.MusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.CoverImages;

public class CoverImageUploader(ICoverImageService service) : ICoverImageUploader
{
    public async Task<string> UploadAsync([NotNull] IFormFile file, string fileId, CancellationToken ct)
    {
        var result = await service.UploadImageAsync(file, fileId, ct);

        return result;
    }
}
