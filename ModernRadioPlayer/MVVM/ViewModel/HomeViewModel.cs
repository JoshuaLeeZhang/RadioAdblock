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
        public ObservableCollection<RadioItem> RadioItems { get; set; }
        private RadioItem? currentRadioItem;

        public HomeViewModel()
        {
            RadioItems = new ObservableCollection<RadioItem>();
            
            _ = LoadRadioItemsAsync();
        }

        private async Task LoadRadioItemsAsync()
        {
            var radioItem1 = await RadioItem.CreateAsync(
                backgroundColor: "#FF7936D7",
                clickCommand: null, // Set to null initially
                name: "CHUM 104.5",
                hardCode: true,
                iconUrl: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJCfS92qzLYTcrhbBY2gREiKUo0mTJPPC-QQ&s",
                streamUrl: "https://14223.live.streamtheworld.com/CHUMFM.mp3");

            // Set the ClickCommand after the RadioItem is created
            radioItem1.ClickCommand = new RelayCommand(param => HandleClick(radioItem1));
            RadioItems.Add(radioItem1);

            //var radioItem2 = await RadioItem.CreateAsync(
            //    backgroundColor: "#FF8359E1",
            //    clickCommand: null, // Set to null initially
            //    name: "CKIS \"KISS 92.5\" Toronto, ON",
            //    displayName: "KISS 92.5");

            //// Set the ClickCommand after the RadioItem is created
            //radioItem2.ClickCommand = new RelayCommand(param => HandleClick(radioItem2));
            //RadioItems.Add(radioItem2);
        }

        private void HandleClick(RadioItem selectedRadioItem)
        {
            if (selectedRadioItem == null || string.IsNullOrEmpty(selectedRadioItem.StreamURL))
            {
                MessageBox.Show("Invalid radio item or stream URL.");
                return;
            }

            if (currentRadioItem != null && currentRadioItem != selectedRadioItem)
            {
                StopStream(currentRadioItem);
            }

            if (currentRadioItem != selectedRadioItem)
            {
                PlayStream(selectedRadioItem);
                currentRadioItem = selectedRadioItem;
            }
        }

        private void PlayStream(RadioItem radioItem)
        {
            radioItem.PlayStream();
        }

        private void StopStream(RadioItem radioItem)
        {
            radioItem.StopStream();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
