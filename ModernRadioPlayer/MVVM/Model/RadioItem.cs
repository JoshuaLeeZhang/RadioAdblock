using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioItem
    {
        public string BackgroundColor { get; set; }
        public ImageSource Favicon { get; set; }
        public string Name { get; set; }
        public string StreamURL { get; set; }
        public ICommand ClickCommand { get; set; }

        private RadioStreamInfo radioStreamInfo;
        private RadioPlaybackService radioPlaybackService;

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

            System.Diagnostics.Debug.WriteLine($"RadioItem has StreamURL: {radioItem.StreamURL}");

            radioItem.radioPlaybackService = new RadioPlaybackService(radioItem.StreamURL);

            return radioItem;
        }

        public void PlayStream()
        {
            radioPlaybackService.PlayStream();
        }

        public void StopStream()
        {
            radioPlaybackService.StopStream();
        }

    }
}
