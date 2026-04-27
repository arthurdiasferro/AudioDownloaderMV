using AudioDownloader.Domain.Entities;

namespace AudioDownloader.Domain.Interfaces;

public interface IAudioScraper
{
    Task<IEnumerable<AudioInfo>> ExtractAudioUrlsAsync(string url, CancellationToken cancellationToken = default);
}