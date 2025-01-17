namespace Noizera.Common.Contracts.QueryResults;

public record ProfileCardQueryResult(
    string Username,
    string PublicId,
    string ProfileType,
    string Name);
