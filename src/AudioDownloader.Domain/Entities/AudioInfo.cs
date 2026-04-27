using System.Text.RegularExpressions;

namespace AudioDownloader.Domain.Entities;

public class AudioInfo
{
    private static readonly Regex InvalidFileNameChars = new(@"[<>:""/\\|?*]", RegexOptions.Compiled);
    
    public string Url { get; }
    public string FileName { get; }

    public AudioInfo(string url)
    {
        Url = ValidateUrl(url);
        FileName = SanitizeFileName(ExtractFileName(url));
    }

    private static string ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be empty", nameof(url));
            
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL format", nameof(url));
            
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException("Only HTTP and HTTPS URLs are allowed", nameof(url));
            
        return url;
    }

    private static string ExtractFileName(string url)
    {
        var uri = new Uri(url);
        var segment = uri.Segments[^1];
        return Uri.UnescapeDataString(segment);
    }

    private static string SanitizeFileName(string fileName)
    {
        fileName = InvalidFileNameChars.Replace(fileName, "_");
        
        if (fileName.Contains(".."))
            throw new ArgumentException("Invalid file name: path traversal detected");
            
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "audio_" + Guid.NewGuid().ToString("N")[..8];
            
        return fileName;
    }
}