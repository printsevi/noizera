namespace Noizera.Shared.Infrastructure.Audio;

public static class AudioHelper
{
    public static List<string> GenerateConversionToFlacCommand(
        string inputFilePath,
        string outputFilePath,
        int sampleRate,
        int targetLufs,
        int truePeak,
        int bitDepth)
    {
        List<string> result = [
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
            outputFilePath
        ];

        return result;
    }

    public static List<string> GenerateConversionToMp3Command(
        string inputFilePath,
        string outputFilePath,
        int targetLufs,
        int truePeak)
    {
        List<string> result = [
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
            outputFilePath
        ];

        return result;
    }
}
