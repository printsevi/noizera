using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.VerificationCodes;
using System.Globalization;

namespace Noizera.Infrastructure.Security.VerificationCodes;

public class VerificationCodeGenerator : IVerificationCodeGenerator
{
    public (string Code, DateTimeOffset ExpireAt) Generate()
    {
        return (GenerateRandomNumber(1000, 9999).ToString(CultureInfo.InvariantCulture), SystemClock.UtcNow.AddMinutes(5));
    }

    private static int GenerateRandomNumber(int min, int max)
    {
        int _min = 1000;
        int _max = 9999;
        Random _rdm = new();
        return _rdm.Next(_min, _max);
    }
}