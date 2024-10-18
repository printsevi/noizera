using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;

namespace Noizera.Shared.Domain.VerificationCodes;

public class VerificationCode : Entity
{
    public string Key { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public bool Invalid { get; private set; }
    public bool Verified { get; private set; }
    public DateTimeOffset ExpireAt { get; private set; }

    VerificationCode(
        string key,
        string code,
        DateTimeOffset expireAt) : base()
    {
        Key = key;
        Code = code;
        ExpireAt = expireAt;
    }

    public static VerificationCode New(string key, IVerificationCodeGenerator codeGenerator)
    {
        var code = codeGenerator.Generate();

        var result = new VerificationCode(key, code.Code, code.ExpireAt);

        result.AddDomainEvent(new VerificationCodeCreatedEvent(key, code.Code));

        return result;
    }

    public bool VerifyAndInvalidateCode(string code)
    {
        Invalid = true;
        Verified = Code.ToLower() == code.ToLower();

        return Verified;
    }

    private VerificationCode() { }
}
