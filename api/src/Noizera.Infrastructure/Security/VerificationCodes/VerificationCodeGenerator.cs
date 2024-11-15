using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.VerificationCodes;
using System.Globalization;

namespace Noizera.Infrastructure.Security.VerificationCodes;

public class VerificationCodeGenerator : IVerificationCodeGenerator
{
    public (string Code, DateTimeOffset ExpireAt) Generate()
        => (GenerateRandomNumber(1000, 9999).ToString(CultureInfo.InvariantCulture), SystemClock.UtcNow.AddMinutes(5));

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "<Pending>")]
    private static int GenerateRandomNumber(int min, int max)
    {
        Random _rdm = new();
        return _rdm.Next(min, max);
    }
}