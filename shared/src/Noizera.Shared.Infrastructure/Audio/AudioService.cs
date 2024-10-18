using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Noizera.Shared.Infrastructure.DataStructure;
using Noizera.Shared.Persistence.Mongo;

namespace Noizera.Shared.Infrastructure.Audio;

public class AudioService(
    MongoDbContext dbContext,
    IOptions<AudioSettings> audioSettings,
    DataStructureProvider dataStructureProvider,
    FfmpegDockerService ffmpegDockerService)
{
    private readonly AudioSettings settings = audioSettings.Value;
    private readonly MongoDbContext db = dbContext;

    public async Task<string> ConvertAndSaveAudioFileToMongoAsync(string fileName, string inputExtension, CancellationToken ct)
    {
        string inputFile = $"{fileName}.{inputExtension}";
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{inputFile}";
        string dockerInputFilePath = $"{dataStructureProvider.DockerAudioPath}/{inputFile}";

        string outputFile = $"output-{fileName}.{settings.OutputExtension}";
        string dockerOutputFilePath = $"{dataStructureProvider.DockerAudioPath}/{outputFile}";
        string outputFilePath = $"{dataStructureProvider.AudioPath}/{outputFile}";

        var command = settings.OutputExtension == "flac" 
            ? AudioHelper.GenerateConversionToFlacCommand(
                dockerInputFilePath,
                dockerOutputFilePath,
                settings.OutputSampleRate,
                settings.TargetLufsInNegative,
                settings.TruePeakInNegative,
                settings.BitDepth)
            : AudioHelper.GenerateConversionToMp3Command(
                dockerInputFilePath,
                dockerOutputFilePath,
                settings.TargetLufsInNegative,
                settings.TruePeakInNegative);

        await ffmpegDockerService.ProcessAsync(
            dataStructureProvider.AudioPath,
            dataStructureProvider.DockerAudioPath,
            command,
            ct);

        if (!File.Exists(outputFilePath))
        {
            throw new Exception($"File {outputFilePath} doesn't exist");
        }

        await TryDeleteOldAudioAsync(fileName, ct).ConfigureAwait(false);

        string mongoFileId;
        using (FileStream outputFileStream = new(outputFilePath, FileMode.Open))
        {
            GridFSUploadOptions options = new()
            {
                Metadata = new BsonDocument { { "fileType", $"audio/{settings.OutputExtension}" } }
            };

            using var stream = await db.AudioBucketWritable.OpenUploadStreamAsync(fileName, options, ct).ConfigureAwait(false);
            mongoFileId = stream.Id.ToString();
            await outputFileStream.CopyToAsync(stream, ct).ConfigureAwait(false);
            await stream.CloseAsync(ct).ConfigureAwait(false);
        }

        return mongoFileId;
    }

    private void DeleteAudioFiles(string fileName)
    {
        string inputFile = $"{fileName}";
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{inputFile}";

        string outputFile = $"output-{fileName}";
        string outputFilePath = $"{dataStructureProvider.AudioPath}/{outputFile}";

        File.Delete(outputFilePath);
        File.Delete(inputFilePath);
    }

    public async Task<Stream> GetAudioAsStreamAsync(string mongoFileName, CancellationToken ct)
    {
        return await db.AudioBucketReadable.OpenDownloadStreamByNameAsync(mongoFileName, new() { Seekable = true }, ct).ConfigureAwait(false);
    }

    public async Task TryDeleteOldAudioAsync(string mongoFileName, CancellationToken ct)
    {
        var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, mongoFileName);
        var fileCursor = await db.AudioBucketReadable.FindAsync(filter, null, ct).ConfigureAwait(false);
        var gridFiles = await fileCursor.ToListAsync(cancellationToken: ct).ConfigureAwait(false);
        foreach (var file in gridFiles)
        {
            await db.AudioBucketWritable.DeleteAsync(file.Id, ct).ConfigureAwait(false);
        }
    }
}
