using ModernRadioPlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRadioPlayer.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }

        public AudioPlayerViewModel AudioPlayerVM { get; set; }

        public HomeViewModel HomeVM { get; set; }

        private object? _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private object? _audioView;

        public object AudioView
        {
            get { return _audioView; }
            set
            {  
                _audioView = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            AudioPlayerVM = new AudioPlayerViewModel();

            CurrentView = HomeVM;
            AudioView = AudioPlayerVM;

            HomeViewCommand = new RelayCommand(o =>
            { 
                CurrentView = HomeVM;
            });
        }
    }
}
