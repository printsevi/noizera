using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Infrastructure.Audio;

namespace Noizera.Infrastructure.Audio;

public class AudioMongoService(AudioService audioService) : IAudioMongoService
{
    public async Task<Stream> GetAudioAsStreamAsync(string mongoFileName, CancellationToken ct)
    {
        return await audioService.GetAudioAsStreamAsync(mongoFileName, ct).ConfigureAwait(false);
    }

    public async Task DeleteAudioAsync(string mongoFileName, CancellationToken ct)
    {
        await audioService.TryDeleteOldAudioAsync(mongoFileName, ct).ConfigureAwait(false);
    }
}
