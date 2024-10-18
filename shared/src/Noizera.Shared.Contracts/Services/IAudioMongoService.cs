namespace Noizera.Shared.Contracts.Services;

public interface IAudioMongoService
{
    Task<Stream> GetAudioAsStreamAsync(string mongoFileName, CancellationToken ct);

    Task DeleteAudioAsync(string mongoFileName, CancellationToken ct);
}
