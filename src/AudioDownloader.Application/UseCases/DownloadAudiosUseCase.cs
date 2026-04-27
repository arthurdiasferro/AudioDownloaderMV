using AudioDownloader.Application.DTOs;
using AudioDownloader.Domain.Entities;
using AudioDownloader.Domain.Interfaces;

namespace AudioDownloader.Application.UseCases;

public class DownloadAudiosUseCase
{
    private readonly IAudioScraper _scraper;
    private readonly IAudioDownloader _downloader;

    public DownloadAudiosUseCase(IAudioScraper scraper, IAudioDownloader downloader)
    {
        _scraper = scraper;
        _downloader = downloader;
    }

    public async Task<IEnumerable<AudioDownloadResult>> ExecuteAsync(
        string url,
        string destinationPath,
        CancellationToken cancellationToken = default)
    {
        var results = new List<AudioDownloadResult>();
        
        try
        {
            var audioInfos = await _scraper.ExtractAudioUrlsAsync(url, cancellationToken);
            
            foreach (var audioInfo in audioInfos)
            {
                try
                {
                    await _downloader.DownloadAsync(audioInfo, destinationPath, cancellationToken);
                    results.Add(new AudioDownloadResult(true, audioInfo.FileName));
                }
                catch (Exception ex)
                {
                    results.Add(new AudioDownloadResult(false, audioInfo.FileName, ex.Message));
                }
            }
        }
        catch (Exception ex)
        {
            // Handle case where AudioInfo creation fails during scraping
            results.Add(new AudioDownloadResult(false, "unknown", $"Failed to process audio information: {ex.Message}"));
        }

        return results;
    }
}