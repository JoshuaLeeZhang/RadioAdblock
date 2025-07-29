# RadioAdblock

A modern WPF radio player with AI-powered advertisement detection that automatically reduces ad volume using machine learning.

![.NET 8](https://img.shields.io/badge/.NET-8.0-blue) ![WPF](https://img.shields.io/badge/UI-WPF-blue) ![ML](https://img.shields.io/badge/ML-TorchSharp-orange)

## Features

- **AI-Powered Ad Detection**: CRNN model classifies audio as music or advertisements
- **Automatic Volume Control**: Reduces ad volume to 10% when detected
- **Real-time Processing**: Live radio stream processing with minimal latency
- **Modern UI**: Clean WPF interface with MVVM architecture

## Technology Stack

- .NET 8.0 Windows (WPF)
- TorchSharp (PyTorch for .NET)
- LibVLCSharp for streaming
- NAudio for audio processing

## Quick Start

1. **Clone and build**
   ```bash
   git clone https://github.com/yourusername/RadioAdblock.git
   cd RadioAdblock
   dotnet restore
   dotnet build --configuration Release
   ```

2. **Run**
   ```bash
   dotnet run --project ModernRadioPlayer
   ```

3. **Usage**
   - Launch the app and wait 35 seconds for ML model initialization
   - Click a radio station to start streaming
   - Ads will be automatically detected and volume reduced

## Configuration

### Model Path
Update the hardcoded path in `AdRemovalService.cs`:
```csharp
crnn.load("C:\\Users\\joshz\\repos\\ModernRadioPlayer\\ModernRadioPlayer\\Models\\model_weights.dat");
```

### Add Radio Stations
Modify `HomeViewModel.cs` to add new stations:
```csharp
var radioItem = await RadioItem.CreateAsync(
    backgroundColor: "#FF7936D7",
    clickCommand: null,
    name: "Station Name",
    hardCode: true,
    iconUrl: "station_icon_url",
    streamUrl: "stream_url"
);
```

## Requirements

- Windows 10/11
- .NET 8.0 Runtime
- Visual Studio 2022 (for development)

## License

MIT License - see [LICENSE](LICENSE) for details. 