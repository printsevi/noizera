using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.ExternalLinks;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Users;

public sealed record UpdateExternalLinkCommand(
    string NewExternalLink,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db,
        IUserRepository userRepository)
        : IRequestHandler<UpdateExternalLinkCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateExternalLinkCommand request, CancellationToken cancellationToken)
        {
            var url = string.IsNullOrWhiteSpace(request.NewExternalLink) ? null : new Uri(request.NewExternalLink);

            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            if (user.Profile.ExternalLinks.Count != 0)
            {
                var result = user.Profile.ExternalLinks.FirstOrDefault()!;
                result.UpdateUrl(url);
                await db.UpdateAsync(result, cancellationToken).ConfigureAwait(false);
            } 
            else
            {
                var result = ExternalLink.New(url, user.Profile);
                await db.InsertAsync(result, cancellationToken).ConfigureAwait(false);
            }

            return Unit.Value;
        }
    }
}

public sealed class UpdateExternalLinkValidator : AbstractValidator<UpdateExternalLinkCommand>
{
    public UpdateExternalLinkValidator()
    {
        _ = RuleFor(x => x.NewExternalLink)
            .MinimumLength(0)
            .MaximumLength(100);
    }
}
