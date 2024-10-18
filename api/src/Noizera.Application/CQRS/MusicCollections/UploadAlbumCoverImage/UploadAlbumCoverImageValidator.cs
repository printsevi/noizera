using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.UploadAlbumCoverImage;

public sealed class UploadAlbumCoverImageValidator : AbstractValidator<UploadAlbumCoverImageCommand>
{
    public UploadAlbumCoverImageValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.AlbumId).NotEmpty();

        _ = RuleFor(x => x.File).NotEmpty();
        _ = RuleFor(x => x.File.Length).NotEmpty().LessThanOrEqualTo(1 * 1024 * 1024)
                .WithMessage("File size is larger than allowed");
        _ = RuleFor(x => x.File.ContentType).Must(x => x.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("File type is incorrect");

    }
}
