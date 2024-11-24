using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Imaging;
using RadioBrowser;
using System.Windows;
using System.Windows.Media;
using ModernRadioPlayer.MVVM.Model;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using ModernRadioPlayer.Core;


namespace ModernRadioPlayer.MVVM.ViewModel
{
    class HomeViewModel : INotifyPropertyChanged
    {
        private readonly RadioBrowserClient radioBrowserClient;

        public RadioStreamInfo radio1 { get; set; } 
        public RadioStreamInfo radio2 { get; set; }
        public RadioStreamInfo radio3 { get; set; }
        public RadioStreamInfo radio4 { get; set; }

        public ObservableCollection<RadioItem> RadioItems { get; set; }

        public HomeViewModel()
        {
            RadioItems = new ObservableCollection<RadioItem>
            {
                new (backgroundColor: "#FF7936D7",
                     clickCommand: new RelayCommand(param => HandleClick(1)),
                     name: "CHUM 104.5",
                     hardCode: true,
                     iconUrl: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJCfS92qzLYTcrhbBY2gREiKUo0mTJPPC-QQ&s",
                     streamUrl: "http://provisioning.streamtheworld.com/pls/CHUMFM.pls"),
                new (backgroundColor: "#FF8359E1",
                     clickCommand: new RelayCommand(param => HandleClick(2)),
                     name: "CKIS \"KISS 92.5\" Toronto, ON",
                     displayName: "KISS 92.5")
            };

        }

        private void HandleClick(int id)
        {
            // Replace this with your desired logic
            System.Windows.MessageBox.Show($"Radio {id} clicked!");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
