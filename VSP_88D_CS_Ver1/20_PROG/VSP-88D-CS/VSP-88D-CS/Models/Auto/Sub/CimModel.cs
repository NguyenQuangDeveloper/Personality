using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using VSP_88D_CS.ViewModels.Auto.Sub;

namespace VSP_88D_CS.Models.Auto.Sub
{
    public class CimModel : INotifyPropertyChanged
    {
        private string _status;
        private Brush _background;

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush Background
        {
            get => _background;
            set
            {
                if (_background != value)
                {
                    _background = value;
                    OnPropertyChanged();
                }
            }
        }
        public CimModel()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class CimSettingItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class CimConfig
    {
        public string DeviceId { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string PassiveMode { get; set; }
    }
}
