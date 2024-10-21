using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Infrastructure.Audio;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

public class AlbumSubmittedEventHandler(
    AudioService audioService,
    AppDbContext db)
    : INotificationHandler<DomainEventNotification<AlbumSubmittedEvent>>
{
    public async Task Handle(DomainEventNotification<AlbumSubmittedEvent> notification, CancellationToken cancellationToken)
    {
        var album = await db.Albums
            .Include(x => x.MusicCollectionSongs)
                    .ThenInclude(x => x.Song)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.AlbumId)
                ?? throw new Exception($"Album {notification.DomainEvent.AlbumId} is not found");

        if (!album.IsProcessable)
        {
            return;
        }

        try
        {
            album.StartProcessing();

            foreach (var song in album.MusicCollectionSongs.Where(x => !x.HasAudioAttached).Select(x => x.Song).ToList())
            {
                await audioService.DownloadOriginalFileAsync(song.PublicId, song.OriginalFileExtension!, cancellationToken).ConfigureAwait(false);

                (var flacLength, var flacBucket) = await audioService.ConvertAndSaveFlacAudioFileToS3Async(song.PublicId, song.OriginalFileExtension!, cancellationToken);
                var duration = await audioService.GetFlacDurationInSecondsAsync(song.PublicId, cancellationToken);
                song.SaveAudioFileToFlacBucket(flacBucket, flacLength, duration);

                (var mp3Length, var mp3Bucket) = await audioService.ConvertAndSaveMp3AudioFileToS3Async(song.PublicId, song.OriginalFileExtension!, cancellationToken);
                song.SaveAudioFileToMp3Bucket(mp3Bucket, mp3Length);

                audioService.DeleteAudioFiles(song.PublicId);
            }

            album.Release();
        }
        catch
        {
            album.StopProcessing();
            throw;
        }
        finally
        {
            db.Update(album);
        }
    }
}
