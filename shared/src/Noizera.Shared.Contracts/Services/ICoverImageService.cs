using Microsoft.AspNetCore.Http;

namespace Noizera.Shared.Contracts.Services;

public interface ICoverImageService
{
    Task<string> UploadImageAsync(IFormFile file, string mongoFileName, CancellationToken ct);

    Task<Stream> GetImageFileAsStreamAsync(string mongoFileName, CancellationToken ct);
}
