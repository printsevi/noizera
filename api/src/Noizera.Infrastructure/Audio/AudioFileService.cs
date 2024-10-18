using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Infrastructure.Audio;
using Noizera.Shared.Infrastructure.DataStructure;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.Audio;

public class AudioFileService(
    IOptions<AudioSettings> audioSettings,
    DataStructureProvider dataStructureProvider) 
    : IAudioFileService
{
    private readonly AudioSettings settings = audioSettings.Value;

    public async Task UploadOriginalAudioFileAsync([NotNull] IFormFile file, string fileName, CancellationToken ct)
    {
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!settings.ValidInputExtensions.Contains(extension))
        {
            throw new AppException($"Invalid file's extension {extension}", ErrorType.Validation);
        }

        string inputFilePath = Path.Combine(dataStructureProvider.AudioPath, $"{fileName}{extension}");
        using (Stream fileStream = new FileStream(inputFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, ct).ConfigureAwait(false);
        }
    }

    public void DeleteOriginalAudioFile(string fileName)
    {
        var inputFilePath = Path.Combine(dataStructureProvider.AudioPath, fileName);
        if (File.Exists(inputFilePath))
        {
            File.Delete(inputFilePath);
        }
    }

    public FileStream GetAudioFileAsStream(string fileName, CancellationToken ct)
    {
        var inputFilePath = Path.Combine(dataStructureProvider.AudioPath, fileName);
        if (!File.Exists(inputFilePath))
        {
            throw new AppException($"Audio file {fileName} not found", ErrorType.NotFound);
        }

        var result = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

        return result;
    }
}
