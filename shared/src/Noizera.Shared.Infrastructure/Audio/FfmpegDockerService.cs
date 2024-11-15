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
        string volumeBind = $"{dataFolderPath}:/{dockerFilesPath}";
        var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "jrottenberg/ffmpeg",
            Name = $"ffmpeg-{Guid.NewGuid()}",
            HostConfig = new HostConfig
            {
                Binds = [volumeBind],
                AutoRemove = true
            },
            Volumes = new Dictionary<string, EmptyStruct>() { { volumeBind, new EmptyStruct() } },
            Cmd = cmd
        }, ct).ConfigureAwait(false);

        bool result = await dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), ct).ConfigureAwait(false);
        if (!result)
        {
            throw new DockerApiException(System.Net.HttpStatusCode.InternalServerError, $"Container {response.ID} not started.");
        }
    }
}
