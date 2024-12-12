using TorchSharp;
using TorchSharp.Transforms;
using static TorchSharp.torch;
using static TorchSharp.torchaudio;
using static TorchSharp.torchaudio.functional;
using static TorchSharp.torchaudio.transforms;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class MFCC
    {
        private readonly MelSpectrogram melSpectrogram;
        private readonly int n_mfcc;
        private readonly Tensor dct_mat;

        public MFCC(int sampleRate = 22050, int numMfcc = 20, int numMels = 128, int nFft = 2048, int hopLength = 512)
        {
            this.n_mfcc = numMfcc;

            melSpectrogram = MelSpectrogram(
                sample_rate: sampleRate,
                n_fft: nFft,
                hop_length: hopLength,
                n_mels: numMels
            );


            this.dct_mat = create_dct(n_mfcc, numMels, norm: DCTNorm.ortho);
        }

        public Tensor forward(Tensor waveform)
        {
            var melSpec = melSpectrogram.forward(waveform);

            melSpec = AmplitudeToDB(melSpec);

            var mfcc = torch.matmul(melSpec.transpose(-1, -2), dct_mat).transpose(-1, -2);

            return mfcc;
        }

        private Tensor AmplitudeToDB(Tensor waveform)
        {
            double multiplier = 10.0; // 10 if stype is "power" else 20.0
            double amin = 1e-10;
            double ref_value = 1.0;
            double db_multiplier = Math.Log10(Math.Max(amin, ref_value));
            double top_db = 80.0;

            return amplitude_to_DB(waveform, multiplier: multiplier, amin: amin, db_multiplier: db_multiplier, top_db: top_db);
        }
    }
}
