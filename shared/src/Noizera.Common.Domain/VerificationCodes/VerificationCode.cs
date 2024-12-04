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

    public static VerificationCode New(string email, string name, [NotNull] IVerificationCodeGenerator codeGenerator)
    {
        var code = codeGenerator.Generate();

        VerificationCode result = new(email, code.Code, code.ExpireAt);

        result.AddDomainEvent(new VerificationCodeCreatedEvent(email, name, code.Code));

        return result;
    }

    public static VerificationCode SubmitContactForm(string email, string name, string topic, string description)
    {
        VerificationCode result = new("ContactFormSubmitted", "", SystemClock.UtcNow);

        result.AddDomainEvent(new ContactFormSubmittedEvent(email, name, topic, description));

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
