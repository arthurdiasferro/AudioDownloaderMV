namespace AudioDownloader.Application.DTOs;

public record AudioDownloadResult(
    bool Success,
    string FileName,
    string? ErrorMessage = null
);