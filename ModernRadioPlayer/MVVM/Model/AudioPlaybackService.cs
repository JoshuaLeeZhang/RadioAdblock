using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class AudioPlaybackService : IDisposable
    {
        private readonly BufferedWaveProvider waveProvider;
        private readonly WaveOutEvent waveOut;
        private CancellationTokenSource playbackCancellationTokenSource;
        private readonly AudioRewindBuffer audioRewindBuffer;

        private readonly int sampleRate;
        private readonly int channels;
        private readonly int bitDepth;

        private bool isPlaying;

        public AudioPlaybackService(int sampleRate, int channels, int bitDepth, AudioRewindBuffer audioRewindBuffer)
        {
            this.sampleRate = sampleRate;
            this.channels = channels;
            this.bitDepth = bitDepth;
                
            this.isPlaying = false;

            waveProvider = new BufferedWaveProvider(new WaveFormat(sampleRate, bitDepth, channels))
            {
                BufferLength = sampleRate * channels * (bitDepth / 8) * 30, // 30 seconds of audio buffer
            };

            waveOut = new WaveOutEvent();
            waveOut.Init(waveProvider);
            this.audioRewindBuffer = audioRewindBuffer;
        }

        public void StartPlayback()
        {
            isPlaying = true;
            playbackCancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => PlaybackLoop(playbackCancellationTokenSource.Token));
        }

        public void StopPlayback()
        {
            isPlaying = false;
            playbackCancellationTokenSource?.Cancel();
            waveOut.Stop();
            waveProvider.ClearBuffer();
        }

        private void PlaybackLoop(CancellationToken cancellationToken)
        {
            // Starts audio playback
            waveOut.Play();

            // Calculate the number of bytes per second based on audio properties
            var bytes_per_second = sampleRate * channels * (bitDepth / 8);
            var chunkDurationInSeconds = 5; // Process audio in 5-second chunks
            var bufferSize = bytes_per_second * chunkDurationInSeconds; // Total bytes to read per chunk

            // Check the amount of data available in the buffer
            int availableData = audioRewindBuffer.AvailableData();

            // Ensure the buffer has at least two times the required chunk size
            while (availableData < 2 * bufferSize)
            {
                Console.WriteLine("Waiting for more data to prefill...");
                Thread.Sleep(1000);
                availableData = audioRewindBuffer.AvailableData();
            }

            // Prefill the waveProvider buffer to avoid stuttering during playback
            PrefillBuffer(bufferSize);

            // Continuously add data to waveProvider buffer until cancelled
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Processing available data...");
                ProcessAvailableData((int)bufferSize, bytes_per_second);
            }
        }

        private void PrefillBuffer(int bufferByteSize)
        {
            var buffer = audioRewindBuffer.ReadFromBuffer(bufferByteSize);
            waveProvider.AddSamples(buffer, 0, bufferByteSize);

            Console.WriteLine("Buffer prefill complete.");
        }   

        private void ProcessAvailableData(int bufferSize, double bytesPerSecond)
        {
            try
            {
                var buffer = audioRewindBuffer.ReadFromBuffer(bufferSize);
                waveProvider.AddSamples(buffer, 0, bufferSize);

                var sleepTime = (bufferSize / bytesPerSecond) * 1000;

                Thread.Sleep((int)sleepTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to waveProvider: {ex.Message}");
            }
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public void Dispose()
        {
            waveOut.Dispose();
            playbackCancellationTokenSource?.Dispose();
        }
    }

}
