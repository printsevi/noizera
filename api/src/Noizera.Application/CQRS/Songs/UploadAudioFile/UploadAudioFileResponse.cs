namespace Noizera.Application.CQRS.Songs.UploadAudioFile;

public sealed record UploadAudioFileResponse(
    string OriginalFileName,
    string ContentType,
    long ContentLength);
