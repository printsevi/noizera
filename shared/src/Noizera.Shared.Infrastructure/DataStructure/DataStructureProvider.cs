using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Noizera.Shared.Infrastructure.DataStructure;

public class DataStructureProvider(IConfiguration configuration, IOptions<DataDirectoryStructure> dataDirectoryStructure)
{
    private readonly DataDirectoryStructure dataStructure = dataDirectoryStructure.Value;
    private readonly bool isDebug = bool.Parse(configuration["IsDebug"]!);
    private readonly string dataFolderPath = configuration["DataFolderPath"]!;

    public string RootDataPath => isDebug ? dataFolderPath : $"/{dataStructure.Root}";

    public string AudioPath => $"{RootDataPath}/{dataStructure.Audio}";

    public string DockerAudioPath => $"/{dataStructure.Root}/{dataStructure.Audio}";
}
