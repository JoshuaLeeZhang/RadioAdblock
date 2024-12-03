using System;

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


        public AudioRewindBuffer(int sampleRate, int channels, int bitDepth, int bufferDuration)
        {
            // Calculate buffer size for bufferDuration seconds of audio
            _bufferSize = sampleRate * channels * (bitDepth / 8) * bufferDuration;
            _buffer = new byte[_bufferSize];
            _writeIndex = 0;
            _availableData = 0;
            _lastReadIndex = 0;
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
    }
}
