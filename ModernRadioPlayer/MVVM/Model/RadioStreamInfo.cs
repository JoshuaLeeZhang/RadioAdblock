using RadioBrowser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RadioBrowser;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioStreamInfo : INotifyPropertyChanged
    {
        private readonly RadioBrowserClient radioBrowserClient;

        private string? name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string? iconUrl;

        public string IconUrl
        {
            get { return iconUrl; }
            set
            {
                if (iconUrl != value)
                {
                    iconUrl = value;
                    OnPropertyChanged(nameof(IconUrl));
                    if (!string.IsNullOrEmpty(iconUrl))
                    {
                        LoadFavicon(iconUrl);
                    }
                }
            }
        }

        private string? streamUrl;

        public string StreamUrl
        {
            get { return streamUrl; }
            set
            {
                if (streamUrl != value)
                {
                    streamUrl = value;
                    OnPropertyChanged(nameof(StreamUrl));
                }
            }
        }

        private ImageSource favicon;

        public ImageSource Favicon
        {
            get { return favicon; }
            set
            {
                if (favicon != value)
                {
                    favicon = value;
                    OnPropertyChanged(nameof(Favicon));
                }
            }
        }




        public RadioStreamInfo(string name, bool hardCode = false, string iconUrl = "", string streamUrl = "", string displayName = "")
        {
            radioBrowserClient = new RadioBrowserClient();

            if (hardCode)
            {
                Name = name;
                IconUrl = iconUrl;
                StreamUrl = streamUrl;
            }
            else
            {

                LoadRadioDataAsync(name).ConfigureAwait(false);

                Name = displayName;
            }

        }

        private async Task LoadRadioDataAsync(string radioName)
        {
            try
            {
                var radioStations = await radioBrowserClient.Search.ByNameAsync(radioName);
                var radioStation = radioStations.FirstOrDefault();

                if (radioStation != null)
                {
                    Name = radioStation.Name ?? "Unknown Station";
                    IconUrl = radioStation.Favicon.AbsoluteUri ?? "C:\\Users\\joshz\\repos\\ModernRadioPlayer\\ModernRadioPlayer\\Images\\QuestionMark.png"; // Default icon URL
                    StreamUrl = radioStation.Url.ToString() ?? string.Empty;
                }
                else
                {
                    Name = "Station Not Found";
                    IconUrl = "C:\\Users\\joshz\\repos\\ModernRadioPlayer\\ModernRadioPlayer\\Images\\QuestionMark.png"; // Default fallback icon
                    StreamUrl = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load radio data: {ex.Message}");
            }
        }


        private void LoadFavicon(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new Exception("Icon URL is null or empty");
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensure it's fully loaded
                bitmap.EndInit();

                Favicon = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load favicon: {ex.Message}");
                Favicon = null; // Clear the favicon on failure
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
