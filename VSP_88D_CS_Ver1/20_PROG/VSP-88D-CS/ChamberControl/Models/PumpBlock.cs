using System.Collections.ObjectModel;
using System.Windows;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl
{
    public class PumpBlock : ViewModelBase
    {
        GasValve _Valve = new() { Name = "Vac. Valve" };
        public GasValve Valve
        {
            get => _Valve;
            set => SetProperty(ref _Valve, value);
        }
        Pump _Pump = new() { Name="Pump", Visible = Visibility.Visible };
        public Pump Pump
        {
            get => _Pump;
            set => SetProperty(ref _Pump, value);
        }
        Pump _Booster = new() { Name = "Booster",Visible=Visibility.Visible};
        public Pump Booster
        {
            get => _Booster;
            set => SetProperty(ref _Booster, value);
        }
    }
}
