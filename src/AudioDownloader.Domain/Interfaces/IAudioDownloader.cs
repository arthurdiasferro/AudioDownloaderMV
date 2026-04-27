using AudioDownloader.Domain.Entities;

namespace AudioDownloader.Domain.Interfaces;

public interface IAudioDownloader
{
    Task DownloadAsync(AudioInfo audioInfo, string destinationPath, CancellationToken cancellationToken = default);
}