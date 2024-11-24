using Docker.DotNet;
using Docker.DotNet.Models;

namespace Noizera.Common.Infrastructure.Audio;

public class FfmpegDockerService(IDockerClient dockerClient)
{
    public async Task ProcessAsync(
        string dataFolderPath,
        string dockerFilesPath,
        IList<string> cmd,
        CancellationToken ct)
    {
        string volumeBind = $"{dataFolderPath}:{dockerFilesPath}";
        var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "jrottenberg/ffmpeg",
            HostConfig = new HostConfig
            {
                Binds = [volumeBind],
                AutoRemove = true
            },
            Cmd = cmd
        }, ct).ConfigureAwait(false);

        _ = await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters(), ct).ConfigureAwait(false);

        var waitResponse = await dockerClient.Containers.WaitContainerAsync(container.ID, ct).ConfigureAwait(false);
        if (waitResponse.StatusCode != 0)
        {
            throw new InvalidOperationException($"FFmpeg process failed with exit code {waitResponse.StatusCode}");
        }
    }
}
