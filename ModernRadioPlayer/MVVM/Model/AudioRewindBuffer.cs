using System;
using System.Threading.Channels;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class AudioRewindBuffer
    {
        private readonly byte[] _buffer;

        private int _writeIndex;
        private readonly int _bufferSize;
        private readonly object _lock = new object();
        private int _availableData;
        private int _lastReadIndex;
        private int _bytes_per_second;
        private int _lastReadIndexPrediction;
        
        private int _bitDepth;


        public AudioRewindBuffer(int sampleRate, int channels, int bitDepth, int bufferDuration)
        {
            // Calculate buffer size for bufferDuration seconds of audio
            _bufferSize = sampleRate * channels * (bitDepth / 8) * bufferDuration;
            _buffer = new byte[_bufferSize];
            _writeIndex = 0;
            _availableData = 0;
            _lastReadIndex = 0;
            _lastReadIndexPrediction = 0;
            _bytes_per_second = sampleRate * channels * (bitDepth / 8);
            _bitDepth = bitDepth;
        }

        public void WriteToBuffer(byte[] data)
        {
            lock (_lock)
            {
                foreach (var b in data)
                {
                    _buffer[_writeIndex] = b;
                    _writeIndex = (_writeIndex + 1) % _bufferSize;

                    if (_availableData < _bufferSize)
                    {
                        _availableData++; // Increment the _availableData and stops at _availableData = _bufferSize
                    }
                }
            }
        }

        public byte[] ReadFromBuffer(int bytesToRead)
        {
            lock (_lock)
            {
                if (bytesToRead > _availableData)
                {
                    throw new ArgumentException("Requested more data than is available in the buffer");
                }

                // Calculate the starting index for reading with the offset
                int readIndex = _lastReadIndex % _bufferSize;

                // Create the result array
                var result = new byte[bytesToRead];

                // Handle circular buffer wrapping
                if (readIndex + bytesToRead <= _bufferSize)
                {
                    Array.Copy(_buffer, readIndex, result, 0, bytesToRead);
                }
                else
                {
                    int firstPartLength = _bufferSize - readIndex;
                    Array.Copy(_buffer, readIndex, result, 0, firstPartLength);
                    Array.Copy(_buffer, 0, result, firstPartLength, bytesToRead - firstPartLength);
                }

                _lastReadIndex += bytesToRead;

                return result;
            }
        }

        internal int AvailableData()
        {
            lock (_lock)
            {
                return _availableData;
            }
        }

        public bool ReadyForPrediction()
        {
            lock (_lock)
            {
                int required_seconds = 10;

                int required_bytes = required_seconds * _bytes_per_second;

                if (_availableData >= required_bytes)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public (byte[] buffer, int readFrom) ReadFromBufferPrediction()
        {
            lock (_lock)
            {
                int seconds_to_read = 10;
                int bytesToRead = seconds_to_read * _bytes_per_second;

                if (bytesToRead > _availableData)
                {
                    throw new ArgumentException("Requested more data than is available in the buffer");
                }
                // Calculate the starting index for reading with the offset
                int readIndex = _lastReadIndexPrediction % _bufferSize;
                // Create the result array
                var result = new byte[bytesToRead];
                // Handle circular buffer wrapping
                if (readIndex + bytesToRead <= _bufferSize)
                {
                    Array.Copy(_buffer, readIndex, result, 0, bytesToRead);
                }
                else
                {
                    int firstPartLength = _bufferSize - readIndex;
                    Array.Copy(_buffer, readIndex, result, 0, firstPartLength);
                    Array.Copy(_buffer, 0, result, firstPartLength, bytesToRead - firstPartLength);
                }
                var readFrom = _lastReadIndexPrediction;
                _lastReadIndexPrediction = (_lastReadIndexPrediction + bytesToRead) % _bufferSize;

                return (result, readFrom);
            }
        }

        public void SetVolume(float volume, int startIndex)
        {
            lock (_lock)
            {
                Console.WriteLine("AD DETECTED");
                volume = Math.Clamp(volume, 0.0f, 1.0f);

                int seconds_to_adjust = 10;
                int byteCount = seconds_to_adjust * _bytes_per_second;
                int bytesPerSample = _bitDepth / 8;

                if (startIndex < 0 || startIndex >= _bufferSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(startIndex), $"Invalid start index. Valid range: 0 to {_bufferSize - 1}");
                }

                int currentIndex = startIndex;
                int adjustedBytes = 0;

                while (adjustedBytes < byteCount)
                {
                    if (currentIndex >= _bufferSize)
                    {
                        currentIndex = 0;
                    }

                    // Combine two bytes into a single 16-bit sample (little-endian)
                    short sample = BitConverter.ToInt16(_buffer, currentIndex);

                    // Adjust the volume
                    sample = (short)(sample * volume);

                    // Write the adjusted sample back to the buffer
                    byte[] adjustedSample = BitConverter.GetBytes(sample);
                    _buffer[currentIndex] = adjustedSample[0];
                    _buffer[currentIndex + 1] = adjustedSample[1];

                    currentIndex += bytesPerSample;
                    adjustedBytes += bytesPerSample;
                }
            }
        }

    }
}
