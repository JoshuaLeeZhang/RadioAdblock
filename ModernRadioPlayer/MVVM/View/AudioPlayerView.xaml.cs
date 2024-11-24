using ModernRadioPlayer.MVVM.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace ModernRadioPlayer.MVVM.View
{

    public partial class AudioPlayerView : UserControl
    {

        public AudioPlayerView()
        {
            InitializeComponent();
            DataContext = new AudioPlayerViewModel();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
