using System.Collections.ObjectModel;

namespace Noizera.Common.Infrastructure.Audio;

public static class AudioHelper
{
    public static ReadOnlyCollection<string> GenerateConversionToFlacCommand(
        string inputFilePath,
        string outputFilePath,
        int sampleRate,
        int targetLufs,
        int truePeak,
        int bitDepth)
    {
        List<string> result = [
            "-y",
            "-loglevel",
            "debug",
            "-i",
            $"{inputFilePath}",
            "-compression_level",
            "12",
            "-af",
            $"loudnorm=I={targetLufs}:TP={truePeak}",
            "-c:a",
            "flac",
            "-sample_fmt",
            $"s{bitDepth}",
            "-ar",
            $"{sampleRate}",
            "-movflags",
            "faststart",
            outputFilePath
        ];

        return new ReadOnlyCollection<string>(result);
    }

    public static ReadOnlyCollection<string> GenerateConversionToMp3Command(
        string inputFilePath,
        string outputFilePath,
        int targetLufs,
        int truePeak)
    {
        List<string> result = [
            "-y",
            "-loglevel",
            "debug",
            "-i",
            $"{inputFilePath}",
            "-f",
            "mp3",
            "-af",
            $"loudnorm=I={targetLufs}:TP={truePeak}",
            "-acodec",
            "libmp3lame",
            "-ab",
            "192000",
            "-ar",
            "44100",
            "-movflags",
            "faststart",
            outputFilePath
        ];

        return new ReadOnlyCollection<string>(result);
    }
}
