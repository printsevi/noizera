using Microsoft.AspNetCore.Http;

namespace Noizera.Shared.Domain.MusicSets;

public interface ICoverImageUploader
{
    Task<string> UploadAsync(IFormFile file, string fileId, CancellationToken ct);
}
