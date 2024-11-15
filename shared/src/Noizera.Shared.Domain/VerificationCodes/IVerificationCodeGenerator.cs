namespace Noizera.Common.Domain.VerificationCodes;

public interface IVerificationCodeGenerator
{
    (string Code, DateTimeOffset ExpireAt) Generate();
}