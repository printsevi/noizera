using Noizera.Shared.Domain.VerificationCodes;

namespace Noizera.Shared.Contracts.Repositories;

public interface IVerificationCodeRepository : IRepository<VerificationCode>
{
    Task<VerificationCode?> GetLatestAsync(string key, CancellationToken ct);
}
