namespace Noizera.Application.CQRS.Auth.Authenticate;

public record AuthenticateResponse(string AccessToken, string RefreshToken);
