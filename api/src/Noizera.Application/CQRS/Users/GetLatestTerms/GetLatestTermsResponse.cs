namespace Noizera.Application.CQRS.Users.GetLatestTerms;

public sealed record GetLatestTermsResponse(string Content, DateTimeOffset EffectiveDate);
