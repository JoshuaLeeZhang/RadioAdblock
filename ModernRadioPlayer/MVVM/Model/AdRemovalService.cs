using TorchSharp;
using static TorchSharp.torch;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class AdRemovalService
    {
        private MFCC? mfcc;
        private CRNN? crnn;
        private AudioRewindBuffer audioRewindBuffer;

        public AdRemovalService(AudioRewindBuffer audioRewindBuffer, int sampleRate = 22050)
        {
            this.audioRewindBuffer = audioRewindBuffer;
            crnn = new CRNN(20);
            crnn.load("C:\\Users\\joshz\\repos\\ModernRadioPlayer\\ModernRadioPlayer\\Models\\model_weights.dat");
            crnn.eval();

            mfcc = new MFCC();

            Task.Run(() =>
            {
                Thread.Sleep(30000); // Sleep for 30 seconds

                FilterAds();
            });
        }

        private void FilterAds()
        {

            while (audioRewindBuffer.ReadyForPrediction())
            {
                if (mfcc == null) throw new Exception("MFCC is null and was not set up correctly");
                if (crnn == null) throw new Exception("CRNN is null and was not set up correctly");

                var (predictionBuffer, readFrom) = audioRewindBuffer.ReadFromBufferPrediction();

                Console.WriteLine("Buffer length: " + predictionBuffer.Length + "    " + "readFrom: " + readFrom);

                Tensor waveform = ByteArrayToTensor.Convert(predictionBuffer);

                Tensor mfccTensor = mfcc.forward(waveform);

                mfccTensor = mfccTensor.view(1, 1, mfccTensor.shape[0], mfccTensor.shape[1]);

                var output = crnn.forward(mfccTensor);

                bool is_music = true;

                if (output.argmax(1).ToInt32() == 1) is_music = false;

                if (!is_music) audioRewindBuffer.SetVolume(0.1f, readFrom);

                Thread.Sleep(10000); // Sleep for 10 second
            }
        }
    }
}