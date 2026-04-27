using AudioDownloader.Application.UseCases;

namespace AudioDownloader.Presentation.Menu;

public class AudioDownloadMenuController
{
    private readonly DownloadAudiosUseCase _downloadUseCase;
    private readonly string _downloadPath;

    public AudioDownloadMenuController(DownloadAudiosUseCase downloadUseCase, string downloadPath)
    {
        _downloadUseCase = downloadUseCase;
        _downloadPath = downloadPath;
    }

    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.Write("Paste URL: ");
        string? url = null;
        try
        {
            url = Console.ReadLine();
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Cannot read input. Returning to main menu...");
            Thread.Sleep(1000);
            return;
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("URL cannot be empty. Returning to main menu...");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            return;
        }

        try
        {
            Console.Clear();
            Console.WriteLine("Searching and downloading audios...\n");

            var results = await _downloadUseCase.ExecuteAsync(url, _downloadPath);
            var successful = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);

            Console.WriteLine($"Download completed: {successful} successful, {failed} failed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nDownload error: {ex.Message}");
        }
    }
}