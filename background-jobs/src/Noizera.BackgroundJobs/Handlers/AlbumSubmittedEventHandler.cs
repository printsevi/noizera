using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Audio;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal class AlbumSubmittedEventHandler(
    AudioService audioService,
    AppDbContext db)
    : INotificationHandler<DomainEventNotification<AlbumSubmittedEvent>>
{
    public async Task Handle(DomainEventNotification<AlbumSubmittedEvent> notification, CancellationToken cancellationToken)
    {
        var album = await db.Albums
            .AsTracking()
            .Include(x => x.MusicSetSongs)
                    .ThenInclude(x => x.Song)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.AlbumId, cancellationToken: cancellationToken).ConfigureAwait(false)
                ?? throw new ArgumentException($"Album {notification.DomainEvent.AlbumId} is not found");

        if (album.IsProcessable)
        {
            foreach (var song in album.MusicSetSongs.Where(x => !x.HasAudioAttached).Select(x => x.Song).ToList())
            {
                await audioService.DownloadOriginalFileAsync(song.PublicId, song.OriginalFileExtension!, cancellationToken).ConfigureAwait(false);

                (long flacLength, string? flacBucket) = await audioService.ConvertAndSaveFlacAudioFileToS3Async(song.PublicId, song.OriginalFileExtension!, cancellationToken).ConfigureAwait(false);
                song.SaveAudioFileToFlacBucket(flacBucket, flacLength);

                (long mp3Length, string? mp3Bucket) = await audioService.ConvertAndSaveMp3AudioFileToS3Async(song.PublicId, song.OriginalFileExtension!, cancellationToken).ConfigureAwait(false);
                double duration = audioService.GetMp3DurationInSecondsAsync(song.PublicId);
                song.SaveAudioFileToMp3Bucket(mp3Bucket, mp3Length, duration);

                audioService.DeleteAudioFiles(song.PublicId);
            }

            album.Release();
        }
    }
}
