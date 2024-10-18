using Microsoft.AspNetCore.Http;

namespace Noizera.Shared.Contracts.Services;

public interface IAudioFileService
{
    Task UploadOriginalAudioFileAsync(IFormFile file, string fileName, CancellationToken ct);

    void DeleteOriginalAudioFile(string fileName);

    FileStream GetAudioFileAsStream(string fileName, CancellationToken ct);
}
