namespace Noizera.Application.CQRS.Users.GetMyUser;

public sealed record GetMyUserResponse(
    string ProfileType,
    string Username,
    string Name,
    IEnumerable<string> ActiveSubscriptions,
    int SongCount,
    bool DayLimitExceeded,
    bool WeekLimitExceeded,
    bool MonthLimitExceeded,
    bool SemiAnnualLimitExceeded);
