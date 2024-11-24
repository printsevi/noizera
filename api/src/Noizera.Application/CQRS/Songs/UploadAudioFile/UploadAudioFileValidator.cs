using FluentValidation;

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

        _ = RuleFor(x => x.File.ContentType).Must(x
            => x.StartsWith("audio/wav", StringComparison.InvariantCultureIgnoreCase)
            || x.StartsWith("audio/aif", StringComparison.InvariantCultureIgnoreCase)
            || x.StartsWith("audio/aiff", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("File type is incorrect");
    }
}
