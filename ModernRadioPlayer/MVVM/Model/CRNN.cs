using TorchSharp;
using TorchSharp.Modules;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace ModernRadioPlayer.MVVM.Model
{
    public class CRNN : Module
    {
        private Conv2d conv1, conv2;
        private BatchNorm2d bn1, bn2;
        private MaxPool2d pool;
        private Dropout dropout;
        private LSTM lstm;
        private Linear fc;
        private int lstmInputSize;

        public CRNN(int inputHeight, int inputChannels = 1, int convChannels = 16, int hiddenSize = 128, int numClasses = 2) : base("CRNN")
        {
            // Define the first convolutional layer with BatchNorm
            conv1 = Conv2d(inputChannels, convChannels, kernel_size: 3, stride: 1, padding: 1);
            bn1 = BatchNorm2d(convChannels);

            // Define the second convolutional layer with BatchNorm
            conv2 = Conv2d(convChannels, convChannels * 2, kernel_size: 3, stride: 1, padding: 1);
            bn2 = BatchNorm2d(convChannels * 2);

            // MaxPooling layer to reduce spatial dimensions by half
            pool = MaxPool2d(kernel_size: 2, stride: 2);

            // Dropout layer to prevent overfitting
            dropout = Dropout(0.05);

            // Calculate the LSTM input size based on inputHeight
            int convOutputHeight = inputHeight / 4; // Two MaxPool layers reduce the height by a factor of 4
            lstmInputSize = (convChannels * 2) * convOutputHeight; // Determine LSTM input size from the output of CNN

            // Define a 2-layer LSTM with the calculated input size
            lstm = LSTM(lstmInputSize, hiddenSize, numLayers: 2, batchFirst: true);

            // Define the fully connected layer for classification
            fc = Linear(hiddenSize, numClasses);

            // Register all components
            RegisterComponents();
        }

        public Tensor forward(Tensor x)
        {
            // Pass input through the first convolutional layer, BatchNorm, ReLU activation, and MaxPool
            x = conv1.forward(x);
            x = bn1.forward(x);
            x = functional.relu(x);
            x = pool.forward(x);

            // Pass input through the second convolutional layer, BatchNorm, ReLU activation, and MaxPool
            x = conv2.forward(x);
            x = bn2.forward(x);
            x = functional.relu(x);
            x = pool.forward(x);

            // Reshape tensor for LSTM
            x = x.permute(0, 3, 1, 2); // Permute dimensions to (batch, time_steps, channels, freq)
            var shape = x.shape;
            int batchSize = (int)shape[0];
            int timeSteps = (int)shape[1];

            // Flatten the last two dimensions (channels and frequency) into one dimension
            x = x.contiguous().view(new long[] { batchSize, timeSteps, -1 });

            // Pass input through the LSTM layer
            var lstmOut = lstm.forward(x).Item1;

            // Select the output of the last time step for classification
            x = lstmOut.index_select(1, torch.tensor(timeSteps - 1));

            // Pass the selected output through the fully connected layer
            x = fc.forward(x.squeeze(1));

            return x;
        }
    }
}
