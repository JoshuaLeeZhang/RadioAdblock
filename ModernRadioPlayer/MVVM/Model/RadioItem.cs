using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioItem
    {
        public string? BackgroundColor { get; set; }
        public ImageSource? Favicon { get; set; }
        public string? Name { get; set; }
        public string? StreamURL { get; set; }
        public ICommand ClickCommand { get; set; }

        private RadioPlaybackService? radioPlaybackService;
        private AudioRewindBuffer? audioRewindBuffer;
        private AudioPlaybackService? audioPlaybackService;
        private AdRemovalService? adRemovalService;
        private RadioItem(string backgroundColor, ICommand clickCommand)
        {
            BackgroundColor = backgroundColor;
            ClickCommand = clickCommand;
        }

        public static async Task<RadioItem> CreateAsync(
            string backgroundColor,
            ICommand clickCommand,
            string name,
            bool hardCode = false,
            string iconUrl = "",
            string streamUrl = "",
            string displayName = "")
        {
            var radioItem = new RadioItem(backgroundColor, clickCommand);

            // Initialize RadioStreamInfo asynchronously
            var radioStreamInfo = await RadioStreamInfo.CreateAsync(name, hardCode, iconUrl, streamUrl, displayName);

            // Assign the initialized properties
            radioItem.Favicon = radioStreamInfo.Favicon;
            radioItem.Name = radioStreamInfo.Name;
            radioItem.StreamURL = radioStreamInfo.StreamUrl;

            Console.WriteLine($"Create RadioItem with name: {radioItem.Name}");

            radioItem.audioRewindBuffer = new AudioRewindBuffer(22050, 1, 16, 60);
            radioItem.radioPlaybackService = new RadioPlaybackService(radioItem.StreamURL, radioItem.audioRewindBuffer); // Writes to audioRewindBuffer
            radioItem.adRemovalService = new AdRemovalService(radioItem.audioRewindBuffer); // Makes ads quieter
            radioItem.audioPlaybackService = new AudioPlaybackService(22050, 1, 16, radioItem.audioRewindBuffer); // Reads from audioRewindBuffer

            return radioItem;
        }

        public void PlayStream()
        {
            audioPlaybackService?.StartPlayback();
        }

        public void StopStream()
        {
            audioPlaybackService?.StopPlayback();
        }

    }
}
