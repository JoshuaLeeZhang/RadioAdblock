using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using LibVLCSharp.Shared;
using System.Net;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioPlaybackService : IDisposable
    {
        private string? streamUrl;
        private readonly LibVLC libVLC;
        private MediaPlayer mediaPlayer;

        public string StreamUrl
        {
            get { return streamUrl; }
            set
            {
                streamUrl = value;

                string actualStreamUrl = ExtractStreamUrl(streamUrl);

                if (!string.IsNullOrEmpty(actualStreamUrl))
                {
                    mediaPlayer.Media = new Media(libVLC, actualStreamUrl, FromType.FromLocation);
                }
                else
                {
                    Debug.WriteLine("Failed to extract stream URL from .pls file.");
                }
            }
        }

        public RadioPlaybackService(string streamUrl)
        {
            LibVLCSharp.Shared.Core.Initialize();
            libVLC = new LibVLC();
            mediaPlayer = new MediaPlayer(libVLC);

            StreamUrl = streamUrl;
        }

        public void PlayStream()
        {
            if (!string.IsNullOrEmpty(StreamUrl))
            {
                if (!mediaPlayer.IsPlaying)
                {
                    mediaPlayer.Play();
                    Debug.WriteLine($"Playing stream from URL: {StreamUrl}");
                }
                else
                {
                    Debug.WriteLine("Stream is already playing.");
                }
            }
            else
            {
                Debug.WriteLine("No stream URL provided.");
            }   
        }

        public void StopStream()
        {
            if (mediaPlayer.IsPlaying)
            {
                mediaPlayer.Stop();
                Debug.WriteLine("Stream stopped.");
            }
            else
            {
                Debug.WriteLine("No stream is currently playing.");
            }
        }
        private string ExtractStreamUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine("The provided URL is null or empty.");
                return string.Empty; // Safely return empty if the input URL is invalid
            }

            if (!url.EndsWith(".pls", StringComparison.OrdinalIgnoreCase))
            {
                return url; // Return the URL directly if it's not a .pls file
            }

            try
            {
                using (var client = new WebClient())
                {
                    string plsContent = client.DownloadString(url);
                    foreach (var line in plsContent.Split('\n'))
                    {
                        if (line.StartsWith("File1=")) // Extract the first audio stream
                        {
                            return line.Substring(6).Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error extracting stream URL: {ex.Message}");
            }

            return string.Empty; // Return empty if no valid URL is found
        }

        public void Dispose()
        {
            mediaPlayer.Dispose();
            libVLC.Dispose();
        }
    }
}
