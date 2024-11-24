using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadioBrowser;


namespace ModernRadioPlayer.MVVM.ViewModel
{
    class AudioPlayerViewModel : INotifyPropertyChanged
    {
        private readonly RadioBrowserClient radioBrowserClient;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AudioPlayerViewModel()
        {
            radioBrowserClient = new RadioBrowserClient();
        }

    }
}
