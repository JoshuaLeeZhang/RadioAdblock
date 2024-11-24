using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace ModernRadioPlayer.MVVM.Model
{
    internal class RadioItem
    {
        public string BackgroundColor { get; set; }
        public ImageSource Favicon { get; set; }
        public string Name { get; set; }
        public string StreamURL { get; set; }
        public ICommand ClickCommand { get; set; }

        private RadioStreamInfo radioStreamInfo;

        public RadioItem(string backgroundColor, ICommand clickCommand, string name, bool hardCode = false, string iconUrl = "", string streamUrl = "", string displayName = "")
        {
            BackgroundColor = backgroundColor;

            radioStreamInfo = new RadioStreamInfo(name, hardCode, iconUrl, streamUrl, displayName);

            Favicon = radioStreamInfo.Favicon;
            Name = radioStreamInfo.Name;
            StreamURL = radioStreamInfo.StreamUrl;

            ClickCommand = clickCommand;
        }

        
    }
}
