using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record UploadProfileImageCommand(
    IFormFile File,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IProfileImageUploader uploader,
        AppDbContext db)
        : IRequestHandler<UploadProfileImageCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            await user.UploadProfileImageAsync(uploader, request.File, cancellationToken).ConfigureAwait(false);

            await db.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}

public sealed class UploadProfileImageValidator : AbstractValidator<UploadProfileImageCommand>
{
    public UploadProfileImageValidator()
    {
        _ = RuleFor(x => x.File).NotEmpty();
        _ = RuleFor(x => x.File.Length).NotEmpty().LessThanOrEqualTo(1 * 1024 * 1024)
                .WithMessage("File size is larger than allowed");
        _ = RuleFor(x => x.File.ContentType).Must(x => x.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
            .WithMessage("File type is incorrect");

    }
}
