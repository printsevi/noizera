namespace Noizera.Application.CQRS.Users.GetSettings;

public sealed record GetSettingsResponse(
    string Name,
    string? Bio,
    string? ImageOriginalName,
    Uri? ExternalLink);
