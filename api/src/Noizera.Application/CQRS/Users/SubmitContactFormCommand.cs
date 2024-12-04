using MediatR;
using Noizera.Common.Domain.VerificationCodes;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record SubmitContactFormCommand(
    string Email,
    string Name,
    string Topic,
    string Description)
    : IRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<SubmitContactFormCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] SubmitContactFormCommand request, CancellationToken cancellationToken)
        {
            VerificationCode result = VerificationCode.SubmitContactForm(request.Email, request.Name, request.Topic, request.Description);
            await db.InsertAsync(result, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
