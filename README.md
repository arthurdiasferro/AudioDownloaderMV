# MV Audio Downloader

[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)

A cross-platform CLI application that scrapes and downloads audio files from web URLs using a menu-driven console interface.

## Features

- HTML audio scraping from web pages
- Batch audio file downloads
- Interactive console menu UI
- Clean Architecture design
- URL validation and file name sanitization

## Prerequisites

- [.NET 6.0+ SDK](https://dotnet.microsoft.com/download)

## Building & Running

```bash
dotnet run
```

## Architecture

```
┌─────────────────────────────────────────┐
│            Presentation                  │
│         (Menu / Console UI)             │
└─────────────────┬───────────────────────┘
                   │
┌─────────────────┴───────────────────────┐
│              Application                │
│          (Use Cases / DTOs)            │
└─────────────────┬───────────────────────┘
                   │
┌─────────────────┴───────────────────────┐
│               Domain                   │
│      (Entities / Interfaces)            │
└─────────────────────────────────────────┘
                   │
┌─────────────────┴───────────────────────┐
│            Infrastructure              │
│   (HtmlAudioScraper, FileDownloader)  │
└───────────────────────────────────────┘
```

## Project Structure

```
src/
├── AudioDownloader.Application/     # Use cases and DTOs
│   ├── DTOs/
│   └── UseCases/
├── AudioDownloader.Domain/        # Core business logic
│   ├── Entities/
│   └── Interfaces/
├── AudioDownloader.Infrastructure/  # External services
│   └── Services/
└── AudioDownloader.Presentation/ # Console UI
    └── Menu/
```

## Usage

1. Run the application with `dotnet run`
2. Select option "1" to search and download audios
3. Enter the URL containing audio files
4. Audio files will be downloaded to your temp directory

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
