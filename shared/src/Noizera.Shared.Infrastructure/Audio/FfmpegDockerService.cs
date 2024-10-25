using Docker.DotNet;
using Docker.DotNet.Models;

namespace Noizera.Shared.Infrastructure.Audio;

public class FfmpegDockerService(IDockerClient dockerClient)
{
    public async Task ProcessAsync(
        string dataFolderPath,
        string dockerFilesPath,
        List<string> cmd,
        CancellationToken ct)
    {
        var volumeBind = $"{dataFolderPath}:/{dockerFilesPath}";
        var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "jrottenberg/ffmpeg",
            Name = $"ffmpeg-{Guid.NewGuid()}",
            HostConfig = new HostConfig
            {
                Binds = new List<string> { volumeBind },
                AutoRemove = true
            },
            Volumes = new Dictionary<string, EmptyStruct>() { { volumeBind, new EmptyStruct() } },
            Cmd = cmd
        }, ct);

        var result = await dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), ct);
    }
}
