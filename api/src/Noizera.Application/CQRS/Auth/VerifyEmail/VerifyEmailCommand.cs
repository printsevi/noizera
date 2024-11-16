using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(
    string Email,
    string Code)
    : IRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        IVerificationCodeRepository verificationCodeRepository)
        : IRequestHandler<VerifyEmailCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var code = await verificationCodeRepository.GetLatestAsync(request.Email, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"A code for {request.Email} not found", ErrorType.NotFound);

            bool isCodeValid = code.VerifyAndInvalidateCode(request.Code);
            await verificationCodeRepository.UpdateAsync(code, cancellationToken).ConfigureAwait(false);

            return !isCodeValid ? throw new AppException($"The code is incorrect", ErrorType.BadRequest) : Unit.Value;
        }
    }
}
