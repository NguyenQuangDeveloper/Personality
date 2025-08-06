using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl
{
    public class Pump : ViewModelBase
    {
        int _Index = 0;
        public int Index
        {
            get => _Index;
            set => SetProperty(ref _Index, value);
        }
        string _Name = string.Empty;
        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        bool _IsOpen = false;
        public bool IsOpen
        {
            get => _IsOpen;
            set => SetProperty(ref _IsOpen, value);
        }
        Visibility _Visibility = Visibility.Collapsed;
        public Visibility Visible
        {
            get => _Visibility;
            set => SetProperty(ref _Visibility, value);
        }

        public ICommand ChangeStatusCommand { get; set; }
        public event Action<object, bool> ChangeStatusEvent = delegate { };

        public Pump()
        {
            ChangeStatusCommand = new RelayCommand(() => { IsOpen = !IsOpen; ChangeStatusEvent?.Invoke(this,IsOpen); });
        }
    }
}