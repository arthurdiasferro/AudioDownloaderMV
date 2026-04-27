using AudioDownloader.Application.UseCases;
using AudioDownloader.Infrastructure.Services;
using AudioDownloader.Presentation.Menu;

var downloadPath = Path.GetTempPath();

var httpClient = new HttpClient();
var networkSecurityValidator = new NetworkSecurityValidator();

var scraper = new HtmlAudioScraper(httpClient);
var downloader = new FileAudioDownloader(httpClient, networkSecurityValidator);
var downloadUseCase = new DownloadAudiosUseCase(scraper, downloader);

var menu = new MenuSystem("MV Audio Downloader");
var downloadController = new AudioDownloadMenuController(downloadUseCase, downloadPath);

menu.AddOption(1, "Search Audios", downloadController.ExecuteAsync);
menu.AddOption(2, "Exit", () => { menu.Exit(); return Task.CompletedTask; });

await menu.ShowAsync();