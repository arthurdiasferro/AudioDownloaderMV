using System.IO;
using System.Net;
using AudioDownloader.Domain.Entities;
using AudioDownloader.Domain.Interfaces;

namespace AudioDownloader.Infrastructure.Services;

public class FileAudioDownloader : IAudioDownloader
{
    private readonly HttpClient _httpClient;
    private readonly INetworkSecurityValidator _networkSecurityValidator;
    private const long MaxFileSizeBytes = 500 * 1024 * 1024;

    public FileAudioDownloader(HttpClient httpClient, INetworkSecurityValidator networkSecurityValidator)
    {
        _httpClient = httpClient;
        _networkSecurityValidator = networkSecurityValidator;
    }

    public async Task DownloadAsync(AudioInfo audioInfo, string destinationPath, CancellationToken cancellationToken = default)
    {
        var fullPath = ValidateAndGetFullPath(audioInfo.FileName, destinationPath);
        
        var uri = new Uri(audioInfo.Url);
        var host = uri.Host;
        if (!_networkSecurityValidator.IsPublicIP(host))
        {
            throw new InvalidOperationException("Download from internal addresses is not allowed");
        }
        
        Directory.CreateDirectory(destinationPath);
        
        // Create temporary file path for atomic operation
        var tempPath = Path.Combine(Path.GetDirectoryName(fullPath)!,$".temp-{Path.GetFileName(fullPath)}");
        
        await using var responseStream = await _httpClient.GetStreamAsync(audioInfo.Url, cancellationToken);
        await using var fileStream = new FileStream(
            tempPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);
        
        long totalBytesRead = 0;
        var buffer = new byte[81920];
        int bytesRead;
        
        while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            totalBytesRead += bytesRead;
            
            // Check if we've exceeded the maximum file size
            if (totalBytesRead > MaxFileSizeBytes)
            {
                // Clean up temporary file
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                
                throw new InvalidOperationException($"File exceeds maximum size of {MaxFileSizeBytes / (1024 * 1024)}MB");
            }
            
            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
        }
        
        // Move temp file to final location atomically
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        
        File.Move(tempPath, fullPath);
    }

    private static string ValidateAndGetFullPath(string fileName, string destinationPath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(destinationPath, fileName));
        var resolvedDestination = Path.GetFullPath(destinationPath);
        
        if (!fullPath.StartsWith(resolvedDestination, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid file path: path traversal detected");
            
        return fullPath;
    }
}