namespace Noizera.Shared.Domain.VerificationCodes;

public interface IVerificationCodeGenerator
{
    (string Code, DateTimeOffset ExpireAt) Generate();
}