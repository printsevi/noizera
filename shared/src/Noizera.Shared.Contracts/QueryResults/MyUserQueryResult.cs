namespace Noizera.Shared.Contracts.QueryResults;

public record MyUserQueryResult(
    string ProfileType,
    string Username,
    string Name,
    IEnumerable<string> ActiveSubscriptions,
    int SongCount);
