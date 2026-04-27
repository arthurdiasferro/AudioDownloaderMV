using AudioDownloader.Domain.Entities;
using AudioDownloader.Domain.Interfaces;
using HtmlAgilityPack;

namespace AudioDownloader.Infrastructure.Services;

public class HtmlAudioScraper : IAudioScraper
{
    private readonly HttpClient _httpClient;

    public HtmlAudioScraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<AudioInfo>> ExtractAudioUrlsAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var html = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var audioInfos = doc.DocumentNode
            .Descendants("audio")
            .Select(a => a.Attributes["src"]?.Value)
            .Where(src => !string.IsNullOrWhiteSpace(src))
            .Select(src => new AudioInfo(src!))
            .ToList();

        return audioInfos;
    }
}