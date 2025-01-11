using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Noizera.Application.CQRS.Songs.UploadAudioFile;

public sealed class UploadAudioFileValidator : AbstractValidator<UploadAudioFileCommand>
{
    public UploadAudioFileValidator()
    {
        _ = RuleFor(x => x.SongId).NotEmpty();

        _ = RuleFor(x => x.File).NotEmpty();

        _ = RuleFor(x => x.File.Length)
            .NotEmpty()
            .GreaterThanOrEqualTo(15 * 1024 * 1024) //a bit smaller than 15mb
            .WithMessage("File size is smaller than allowed");

        _ = RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(300 * 1024 * 1024) //a bit greater than 200mb
            .WithMessage("File size is larger than allowed");

        _ = RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required.")
            .Must(BeValidWavFile)
            .WithMessage("The file must be a valid WAV audio file.");
    }

    private static bool BeValidWavFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        // Check file extension
        var allowedExtensions = new[] { ".wav" };
        var fileExtension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(fileExtension.ToLower(System.Globalization.CultureInfo.CurrentCulture)))
            return false;

        // Check MIME type
        if (!string.IsNullOrEmpty(file.ContentType) &&
            !file.ContentType.Equals("audio/wav", StringComparison.OrdinalIgnoreCase) &&
            !file.ContentType.Equals("audio/x-wav", StringComparison.OrdinalIgnoreCase))
            return false;

        // Check file content (WAV file signature)
        using (var reader = new BinaryReader(file.OpenReadStream()))
        {
            // Check if file is long enough to contain WAV header
            if (reader.BaseStream.Length < 12)
                return false;

            // Check RIFF header
            if (new string(reader.ReadChars(4)) != "RIFF")
                return false;

            // Skip file size
            reader.ReadInt32();

            // Check WAVE header
            if (new string(reader.ReadChars(4)) != "WAVE")
                return false;
        }

        return true;
    }
}
