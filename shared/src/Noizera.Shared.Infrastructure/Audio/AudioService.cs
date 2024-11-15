using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using NAudio.Wave;
using Noizera.Common.Infrastructure.DataStructure;
using Noizera.Common.Persistence.S3;

namespace Noizera.Common.Infrastructure.Audio;

public class AudioService(
    S3Context s3Context,
    IOptions<AudioSettings> audioSettings,
    DataStructureProvider dataStructureProvider,
    FfmpegDockerService ffmpegDockerService)
{
    private readonly AudioSettings settings = audioSettings.Value;
    private readonly S3Context s3 = s3Context;

    public async Task<(long FlacLength, string FlacBucket)> ConvertAndSaveFlacAudioFileToS3Async(string fileId, string inputExtension, CancellationToken ct)
    {
        string inputFile = $"{fileId}{inputExtension}";
        string dockerInputFilePath = $"{dataStructureProvider.DockerAudioPath}/{inputFile}";

        string outputFlacFile = $"output-flac-{fileId}.flac";
        string dockerOutputFlacFilePath = $"{dataStructureProvider.DockerAudioPath}/{outputFlacFile}";
        string outputFlacFilePath = $"{dataStructureProvider.AudioPath}/{outputFlacFile}";

        var flacCommand = AudioHelper.GenerateConversionToFlacCommand(
                dockerInputFilePath,
                dockerOutputFlacFilePath,
                settings.OutputSampleRate,
                settings.TargetLufsInNegative,
                settings.TruePeakInNegative,
                settings.BitDepth);

        await ffmpegDockerService.ProcessAsync(
            dataStructureProvider.AudioPath,
            dataStructureProvider.DockerAudioPath,
            flacCommand,
            ct).ConfigureAwait(false);

        long flacLength;
        string flacBucket;
        using (FileStream outputFileStream = new(outputFlacFilePath, FileMode.Open))
        {
            (flacLength, flacBucket) = await s3.UploadFlacAudioAsync(fileId, "audio/flac", outputFileStream, ct).ConfigureAwait(false);
        }

        return (flacLength, flacBucket);
    }

    public async Task<(long Mp3Length, string Mp3Bucket)> ConvertAndSaveMp3AudioFileToS3Async(string fileId, string inputExtension, CancellationToken ct)
    {
        string inputFile = $"{fileId}{inputExtension}";
        string dockerInputFilePath = $"{dataStructureProvider.DockerAudioPath}/{inputFile}";

        string outputMp3File = $"output-mp3-{fileId}.mp3";
        string dockerOutputMp3FilePath = $"{dataStructureProvider.DockerAudioPath}/{outputMp3File}";
        string outputMp3FilePath = $"{dataStructureProvider.AudioPath}/{outputMp3File}";

        var mp3Command = AudioHelper.GenerateConversionToMp3Command(
                dockerInputFilePath,
                dockerOutputMp3FilePath,
                settings.TargetLufsInNegative,
                settings.TruePeakInNegative);

        await ffmpegDockerService.ProcessAsync(
            dataStructureProvider.AudioPath,
            dataStructureProvider.DockerAudioPath,
            mp3Command,
            ct).ConfigureAwait(false);

        long mp3Length;
        string mp3Bucket;
        using (FileStream outputFileStream = new(outputMp3FilePath, FileMode.Open))
        {
            (mp3Length, mp3Bucket) = await s3.UploadMp3AudioAsync(fileId, "audio/mpeg", outputFileStream, ct).ConfigureAwait(false);
        }

        return (mp3Length, mp3Bucket);
    }

    public async Task DownloadOriginalFileAsync(string fileId, string originalExtension, CancellationToken ct)
    {
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{fileId}{originalExtension}";

        using var response = await s3.GetOriginalAudioFileAsync(fileId, ct).ConfigureAwait(false);
        using var responseStream = response.ResponseStream;
        using var fileStream = File.Create(inputFilePath);
        await responseStream.CopyToAsync(fileStream, ct).ConfigureAwait(false);
    }

    public double GetMp3DurationInSecondsAsync(string fileId)
    {
        string outputMp3File = $"output-mp3-{fileId}.mp3";
        Mp3FileReader reader = new($"{dataStructureProvider.AudioPath}/{outputMp3File}");

        return reader.TotalTime.TotalSeconds;
    }

    public void DeleteAudioFiles(string fileId)
    {
        string inputFile = $"{fileId}";
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{inputFile}";

        string outputFlacFile = $"output-flac-{fileId}";
        string outputFlacFilePath = $"{dataStructureProvider.AudioPath}/{outputFlacFile}";

        string outputMp3File = $"output-mp3-{fileId}";
        string outputMp3FilePath = $"{dataStructureProvider.AudioPath}/{outputMp3File}";

        File.Delete(outputFlacFilePath);
        File.Delete(outputMp3FilePath);
        File.Delete(inputFilePath);
    }
}
