using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.SecretTokens;

public class SecretToken : Entity
{
    public string Token { get; private set; } = null!;
    public SecretTokenType TokenType { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; } = null!;
    public DateTimeOffset ExpireAt { get; }

    SecretToken(
        string token,
        User user,
        SecretTokenType tokenType,
        DateTimeOffset expireAt)
    {
        Token = token;
        UserId = user.Id;
        TokenType = tokenType;
        ExpireAt = expireAt;
    }

    public bool IsValid => !IsRevoked && SystemClock.UtcNow < ExpireAt;

    public static SecretToken NewRefreshToken([NotNull] ISecretTokenGenerator tokenGenerator, User user)
    {
        var token = tokenGenerator.GenerateRefreshToken();

        return new(token.Token, user, SecretTokenType.Refresh, token.ExpireAt);
    }

    public static SecretToken NewResetToken([NotNull] ISecretTokenGenerator tokenGenerator, User user)
    {
        var token = tokenGenerator.GenerateResetToken();
        var result = new SecretToken(token.Token, user, SecretTokenType.Reset, token.ExpireAt);

        result.AddDomainEvent(new ResetTokenCreatedEvent(user.Email, user.Id, token.Token));

        return result;
    }

    public void RevokeToken()
    {
        IsRevoked = true;
    }

    private SecretToken() { }
}
