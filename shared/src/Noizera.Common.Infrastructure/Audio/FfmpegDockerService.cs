using Docker.DotNet;
using Docker.DotNet.Models;
using System.Net.Sockets;

namespace Noizera.Common.Infrastructure.Audio;

public class FfmpegDockerService(IDockerClient dockerClient)
{
    public async Task ProcessAsync(
        string dataFolderPath,
        string dockerFilesPath,
        IList<string> cmd,
        CancellationToken ct)
    {
        //CreateContainerResponse? container = null;
        try
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
                var (stdout, stderr) = await GetContainerLogsAsync(container.ID, ct).ConfigureAwait(false);
                throw new InvalidOperationException($"FFmpeg process failed with exit code {waitResponse.StatusCode}. Logs: {stdout}. Errors: {stderr}");
            }
        }
        catch (SocketException ex)
        {
            throw new InvalidOperationException("Error connecting to Docker daemon.", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while processing the container.", ex);
        }
        //finally
        //{
        //    if (container != null)
        //    {
        //        await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true }, ct).ConfigureAwait(false);
        //    }
        //}
    }

    private async Task<(string stdout, string stderr)> GetContainerLogsAsync(string containerId, CancellationToken cancellationToken)
    {
        ContainerLogsParameters parameters = new()
        {
            ShowStdout = true,
            ShowStderr = true,
            Follow = false
        };

        var stream = await dockerClient.Containers.GetContainerLogsAsync(
            containerId,
            false,
            parameters,
            cancellationToken
        ).ConfigureAwait(false);

        (string stdout, string stderr) = await stream.ReadOutputToEndAsync(cancellationToken).ConfigureAwait(false);

        return (stdout, stderr);
    }
}
