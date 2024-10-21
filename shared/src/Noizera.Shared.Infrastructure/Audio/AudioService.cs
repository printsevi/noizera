using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Noizera.Shared.Infrastructure.DataStructure;
using Noizera.Shared.Persistence.S3;
using System.Diagnostics;
using Xabe.FFmpeg;

namespace Noizera.Shared.Infrastructure.Audio;

public class AudioService(
    S3Context s3Context,
    IOptions<AudioSettings> audioSettings,
    IOptions<S3BucketSettings> s3BucketSettings,
    DataStructureProvider dataStructureProvider,
    FfmpegDockerService ffmpegDockerService)
{
    private readonly AudioSettings settings = audioSettings.Value;
    private readonly S3BucketSettings s3Settings = s3BucketSettings.Value;
    private readonly S3Context s3 = s3Context;

    public async Task<(long FlacLength, string FlacBucket)> ConvertAndSaveFlacAudioFileToS3Async(string fileId, string inputExtension, CancellationToken ct)
    {
        string inputFile = $"{fileId}.{inputExtension}";
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
            ct);

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
        string inputFile = $"{fileId}.{inputExtension}";
        string dockerInputFilePath = $"{dataStructureProvider.DockerAudioPath}/{inputFile}";

        string outputMp3File = $"output-mp3-{fileId}.flac";
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
            ct);

        long mp3Length;
        string mp3Bucket;
        using (FileStream outputFileStream = new(outputMp3FilePath, FileMode.Open))
        {
            (mp3Length, mp3Bucket) = await s3.UploadMp3AudioAsync(fileId, "audio/mp3", outputFileStream, ct).ConfigureAwait(false);
        }

        return (mp3Length, mp3Bucket);
    }

    public async Task DownloadOriginalFileAsync(string fileId, string originalExtension, CancellationToken ct)
    {
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{fileId}.{originalExtension}";
        using (GetObjectResponse response = await s3.GetOriginalAudioFileAsync(fileId, ct))
        {
            await using (Stream responseStream = response.ResponseStream)
            await using (var fileStream = File.Create(inputFilePath))
            {
                await responseStream.CopyToAsync(fileStream);
            }
        }
    }

    public async Task<double> GetFlacDurationInSecondsAsync(string fileId, CancellationToken ct)
    {
        string outputFlacFile = $"output-flac-{fileId}";
        var mediaInfo = await FFmpeg.GetMediaInfo($"{dataStructureProvider.AudioPath}/{outputFlacFile}", ct);

        return mediaInfo.Duration.TotalSeconds;
    }

    public void DeleteAudioFiles(string fileId)
    {
        string inputFile = $"{fileId}";
        string inputFilePath = $"{dataStructureProvider.AudioPath}/{inputFile}";

        string outputFlacFile = $"output-flac-{fileId}";
        string outputFlacFilePath = $"{dataStructureProvider.AudioPath}/{outputFlacFile}";

        string outputMp3File = $"output-mp3-{fileId}";
        string outputMp3FilePath = $"{dataStructureProvider.AudioPath}/{outputFlacFile}";

        File.Delete(outputFlacFilePath);
        File.Delete(outputMp3FilePath);
        File.Delete(inputFilePath);
    }
}
