using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class StreamUrlExtractor
    {

        public string ExtractStreamUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("The provided URL is null or empty.");
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
                Console.WriteLine($"Error extracting stream URL: {ex.Message}");
            }

            return string.Empty; // Return empty if no valid URL is found
        }
    }
}
