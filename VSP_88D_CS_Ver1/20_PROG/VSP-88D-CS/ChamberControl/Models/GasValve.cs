using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl
{
    public class GasValve : ViewModelBase
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
        public ICommand ChangeStatusCommand { get; set; }
        public event Action<object, bool> ChangeStatusEvent = delegate { };

        public GasValve()
        {
            ChangeStatusCommand = new RelayCommand(() => { IsOpen = !IsOpen; ChangeStatusEvent?.Invoke(this,IsOpen); });
        }
    }
}
