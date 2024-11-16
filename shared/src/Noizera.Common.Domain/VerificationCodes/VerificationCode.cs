using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Events;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.VerificationCodes;

public class VerificationCode : Entity
{
    public string Key { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public bool Invalid { get; private set; }
    public bool Verified { get; private set; }
    public DateTimeOffset ExpireAt { get; private set; }

    private VerificationCode(
        string key,
        string code,
        DateTimeOffset expireAt) : base()
    {
        Key = key;
        Code = code;
        ExpireAt = expireAt;
    }

    public static VerificationCode New(string key, [NotNull] IVerificationCodeGenerator codeGenerator)
    {
        var code = codeGenerator.Generate();

        VerificationCode result = new(key, code.Code, code.ExpireAt);

        result.AddDomainEvent(new VerificationCodeCreatedEvent(key, code.Code));

        return result;
    }

    public bool VerifyAndInvalidateCode(string code)
    {
        Invalid = true;
        Verified = Code.Equals(code, StringComparison.OrdinalIgnoreCase);

        return Verified;
    }

    private VerificationCode() { }
}
