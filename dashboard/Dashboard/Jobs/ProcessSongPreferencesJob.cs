using Microsoft.EntityFrameworkCore;
using Noizera.Common.Domain.ListeningHistories;
using Noizera.Common.Domain.SongPreferences;
using Noizera.Common.Persistence.SQL;

namespace Dashboard.Jobs;

public class ProcessSongPreferencesJob(
    AppDbContext db,
    ILogger<ProcessSongSimilaritiesJob> logger)
{
    public async Task ProcessAsync()
    {
        var pairs = await (from t1 in db.Songs
                           from t2 in db.Songs
                           where t1.Id != t2.Id && t1.Danceability.HasValue && t2.Danceability.HasValue
                               && t1.Energy.HasValue && t2.Energy.HasValue //Add if song is released
                               && !db.SongSimilarities.Any(ts => (ts.FirstSongId == t1.Id && ts.SecondSongId == t2.Id) ||
                                                                       (ts.FirstSongId == t2.Id && ts.SecondSongId == t1.Id))
                           select new { Song1Id = t1.Id, Song2Id = t2.Id })
                            .Select(pair => new Tuple<Guid, Guid>(pair.Song1Id, pair.Song2Id))
                            .Take(100)
                            .ToListAsync();

        var histories = await db.ListeningHistories
            .Where(lh => !db.SongPreferences.Any(sp => sp.UserId != lh.ListenerUserId && sp.SongId != lh.SongId))
            .Take(100)
            .ToListAsync();

        var outdatedPreferences = new List<SongPreference>();
        if (histories.Count < 100)
        {
            outdatedPreferences = await db.SongPreferences
                .OrderBy(x => x.LastProcessedOn)
                .Take(100 - histories.Count)
                .ToListAsync();
            var additionalHistories = await db.ListeningHistories
                .Where(lh => outdatedPreferences.Any(op => op.UserId == lh.ListenerUserId && op.SongId == lh.SongId))
                .ToListAsync();
            histories.AddRange(additionalHistories);
        }

        if (histories.Count == 0)
        {
            return;
        }

        foreach (var history in histories)
        {
            var userListeningTimeInSeconds = await db.ListeningHistories
                .Where(x => x.ListenerUserId == history.ListenerUserId).SumAsync(x => x.ListeningTimeInSeconds);

            var score = CalculateCompositeScore(history, userListeningTimeInSeconds);

            var oudatedPreference = outdatedPreferences.FirstOrDefault(x => x.UserId == history.ListenerUserId && x.SongId == history.SongId);
            if (oudatedPreference is null)
            {
                var songPreferences = SongPreference.Create(history.ListenerUserId, history.SongId, score);
                await db.SongPreferences.AddAsync(songPreferences);
            }
            else
            {
                oudatedPreference.Process(score);
                db.SongPreferences.Update(oudatedPreference);
            }

            await db.SaveChangesAsync();
        }
    }

    private float CalculateCompositeScore(ListeningHistory history, int userListeningTimeInSeconds)
    {
        var playCountWeight = 1.0f;
        var skipRateWeight = -2.0f;
        var recencyWeight = 0.5f;
        var listeningWeight = 2f;

        var skipRate = history.SkipCount > 0 ? history.SkipCount / history.PlayCount : 0f;

        var daysSinceLastPlayed = 0;
        if (history.LastPlayed.HasValue)
        {
            daysSinceLastPlayed = (int)(DateTimeOffset.UtcNow - history.LastPlayed).Value.TotalDays;
        }
        var recencyScore = daysSinceLastPlayed > 0 ? 1f / daysSinceLastPlayed : 1f;

        var listeningScore = history.ListeningTimeInSeconds / userListeningTimeInSeconds;

        var score = (history.PlayCount * playCountWeight) +
                       (skipRate * skipRateWeight) +
                       (recencyScore * recencyWeight) +
                       (listeningScore * listeningWeight);

        return score;
    }
}
