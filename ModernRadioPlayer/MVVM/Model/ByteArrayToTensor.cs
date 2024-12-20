using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorchSharp;
using static TorchSharp.torch;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class ByteArrayToTensor
    {
        public static Tensor Convert(byte[] byteArray, int sampleRate = 22050, int channels = 1, int bitDepth = 16)
        {
            // Calculate bytes per sample
            int bytesPerSample = bitDepth / 8;

            // Ensure byte array length is divisible by bytes per sample
            if (byteArray.Length % bytesPerSample != 0)
            {
                throw new ArgumentException("Byte array length is not a multiple of bytes per sample.");
                // Look into -1 or +1 to byteArray to fix
            }

            // Convert byte array to an array of floats
            int numSamples = byteArray.Length / bytesPerSample;
            float[] floatSamples = new float[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                short sample = BitConverter.ToInt16(byteArray, i * bytesPerSample);
                floatSamples[i] = sample / (float)short.MaxValue; 
            }

            // Convert float array to Torch tensor
            var tensor = torch.tensor(floatSamples);
            return tensor;
        }
    }
}
