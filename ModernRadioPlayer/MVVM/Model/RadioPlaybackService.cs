using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using LibVLCSharp.Shared;
using System.Net;
using System.Threading.Channels;
using NAudio.Wave;
using System.Runtime.InteropServices;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioPlaybackService : IDisposable
    {
        private string? streamUrl;
        private readonly LibVLC libVLC;
        private readonly MediaPlayer? mediaPlayer;
        private readonly AudioRewindBuffer? audioRewindBuffer;
        private readonly AudioPlaybackService? audioPlaybackService;

        private readonly int sampleRate;
        private readonly int channels;
        private readonly int bitDepth;

        public string? StreamUrl
        {
            get { return streamUrl; }
            set
            {
                streamUrl = value;

                var streamUrlExtractor = new StreamUrlExtractor();
                string actualStreamUrl = streamUrlExtractor.ExtractStreamUrl(streamUrl);

                if (!string.IsNullOrEmpty(actualStreamUrl))
                {
                    mediaPlayer.Media = new Media(libVLC, actualStreamUrl, FromType.FromLocation);
                }
                else
                {
                    Console.WriteLine("Failed to extract stream URL from .pls file.");
                }
            }
        }

        public RadioPlaybackService(string streamUrl, AudioRewindBuffer audioRewindBuffer, int sampleRate = 22050, int channels = 1, int bitDepth = 16)
        {
            LibVLCSharp.Shared.Core.Initialize();
            libVLC = new LibVLC();
            mediaPlayer = new MediaPlayer(libVLC);

            this.sampleRate = sampleRate;
            this.channels = channels;
            this.bitDepth = bitDepth;
            this.audioRewindBuffer = audioRewindBuffer;
            
            audioPlaybackService = new AudioPlaybackService(sampleRate, channels, bitDepth, audioRewindBuffer);

            StreamUrl = streamUrl;

            mediaPlayer.SetAudioCallbacks(SaveToBuffer, null, null, null, null);
            mediaPlayer.SetAudioFormat("S16N", (uint)sampleRate, (uint)channels);

            mediaPlayer.Play(); //This is necessary to start the audio callback SaveToBuffer, but does not actually play the stream
        }

        private void SaveToBuffer(IntPtr data, IntPtr samples, uint count, long pts)
        {
            try
            {
                int bytesPerSample = bitDepth / 8;
                int bufferLength = (int)(count * bytesPerSample);

                byte[] buffer = new byte[bufferLength];
                Marshal.Copy(samples, buffer, 0, bufferLength);

                audioRewindBuffer?.WriteToBuffer(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving audio to buffer: {ex.Message}");
            }
        }

        public void Dispose()
        {
            mediaPlayer.Dispose();
            audioPlaybackService.Dispose();
            libVLC.Dispose();
        }
    }
}
