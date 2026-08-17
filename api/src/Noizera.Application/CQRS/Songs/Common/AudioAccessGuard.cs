using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Domain.Songs;

namespace Noizera.Application.CQRS.Songs.Common;

/// <summary>
/// Canonical audio rendition names as they appear on the wire.
/// </summary>
public static class AudioType
{
    public const string Original = "original";
    public const string Flac = "audio/flac";
    public const string Mpeg = "audio/mpeg";
}

/// <summary>
/// Single server-side authority over who may receive which audio rendition, and how large it is.
/// </summary>
/// <remarks>
/// Entitlement used to be decided only in the browser — the player asked for FLAC when the user
/// had a subscription and MP3 otherwise — so any caller could simply ask for FLAC and receive
/// the paid, lossless rendition. Likewise the original WAV master was reachable anonymously.
/// <para>The rules enforced here are:</para>
/// <list type="bullet">
///   <item><description><b>original</b> — the artist's master. Owner only.</description></item>
///   <item><description><b>audio/flac</b> — the paid rendition. Requires an active subscription (owners always get their own).</description></item>
///   <item><description><b>audio/mpeg</b> — the free rendition. Anyone, for a released song.</description></item>
/// </list>
/// <para>
/// Content length is resolved from the stored <see cref="Song"/> rather than trusted from the
/// request, so a caller cannot use it to steer range reads.
/// </para>
/// </remarks>
public sealed class AudioAccessGuard(
    ISongRepository songRepository,
    IUserRepository userRepository)
{
    public sealed record AudioAccess(Song Song, long ContentLength);

    public async Task<AudioAccess> AuthorizeAsync(string songPublicId, string audioType, Guid? userId, CancellationToken ct)
    {
        var song = await songRepository.GetAsync(songPublicId, ct).ConfigureAwait(false)
            ?? throw new AppException("The song is not found", ErrorType.NotFound);

        bool isOwner = userId.HasValue && song.OwnerId == userId.Value;

        return audioType switch
        {
            AudioType.Original => AuthorizeOriginal(song, isOwner),
            AudioType.Flac => await AuthorizeFlacAsync(song, userId, isOwner, ct).ConfigureAwait(false),
            AudioType.Mpeg => AuthorizeMpeg(song, isOwner),
            _ => throw new AppException($"Content type is undefined {audioType}", ErrorType.Validation),
        };
    }

    private static AudioAccess AuthorizeOriginal(Song song, bool isOwner)
    {
        // The uploaded master is never distributed — it is the artist's own asset.
        if (!isOwner)
        {
            throw new AppException("The original audio file is available to its owner only.", ErrorType.Authorization);
        }

        return new(song, Require(song.OriginalContentLength, AudioType.Original, song.PublicId));
    }

    private async Task<AudioAccess> AuthorizeFlacAsync(Song song, Guid? userId, bool isOwner, CancellationToken ct)
    {
        if (!isOwner)
        {
            EnsureReleased(song);

            if (!userId.HasValue)
            {
                throw new AppException("Lossless audio requires an active subscription.", ErrorType.Authorization);
            }

            var user = await userRepository.GetWithSubscriptionsAsync(userId.Value, ct).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            if (!user.HasAnyActiveSubscription)
            {
                throw new AppException("Lossless audio requires an active subscription.", ErrorType.Authorization);
            }
        }

        return new(song, Require(song.FlacContentLength, AudioType.Flac, song.PublicId));
    }

    private static AudioAccess AuthorizeMpeg(Song song, bool isOwner)
    {
        if (!isOwner)
        {
            EnsureReleased(song);
        }

        return new(song, Require(song.MpegContentLength, AudioType.Mpeg, song.PublicId));
    }

    private static void EnsureReleased(Song song)
    {
        if (!song.IsPublic)
        {
            throw new AppException("The song is not found", ErrorType.NotFound);
        }
    }

    private static long Require(long? contentLength, string audioType, string songPublicId)
        => contentLength is > 0
            ? contentLength.Value
            : throw new AppException($"No {audioType} audio is available for song {songPublicId}.", ErrorType.NotFound);
}
