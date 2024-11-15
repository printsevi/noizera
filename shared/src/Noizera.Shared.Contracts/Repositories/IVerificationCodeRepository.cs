using Noizera.Common.Domain.VerificationCodes;

namespace Noizera.Common.Contracts.Repositories;

public interface IVerificationCodeRepository : IRepository<VerificationCode>
{
    Task<VerificationCode?> GetLatestAsync(string key, CancellationToken ct);
}
