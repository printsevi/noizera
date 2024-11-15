using Microsoft.EntityFrameworkCore;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Domain.SongSimilarities;
using Noizera.Common.Persistence.SQL;

namespace Dashboard.Jobs;

public class ProcessSongSimilaritiesJob(AppDbContext db, ILogger<ProcessSongSimilaritiesJob> logger)
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

        if (pairs.Count == 0)
        {
            return;
        }

        foreach (var pair in pairs)
        {
            var song1 = await db.Songs.FirstOrDefaultAsync(x => x.Id == pair.Item1);
            var song2 = await db.Songs.FirstOrDefaultAsync(x => x.Id == pair.Item2);

            if (song1 is null || song2 is null)
            {
                return;
            }

            var score = CalculateCosineSimilarity(song1!, song2!);

            var trackSimilarity = SongSimilarity.Create(song1.Id, song2.Id, score);

            await db.SongSimilarities.AddAsync(trackSimilarity);
            await db.SaveChangesAsync();
        }
    }

    private float CalculateCosineSimilarity(Song song1, Song song2)
    {
        var dotProduct = (song1.Energy * song2.Energy) + (song1.Danceability * song2.Danceability);
        var magnitude1 = Math.Sqrt(Math.Pow(song1.Energy!.Value, 2) + Math.Pow(song1.Danceability!.Value, 2));
        var magnitude2 = Math.Sqrt(Math.Pow(song2.Energy!.Value, 2) + Math.Pow(song2.Danceability!.Value, 2));

        if (magnitude1 == 0 || magnitude2 == 0)
        {
            return 0.0f;
        }

        return dotProduct!.Value / (float)(magnitude1 * magnitude2);
    }
}
