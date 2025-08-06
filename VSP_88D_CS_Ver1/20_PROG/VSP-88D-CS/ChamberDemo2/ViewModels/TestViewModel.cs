using ChamberControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberDemo2.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        ChamberViewModel _Chamber;
        public ChamberViewModel Chamber
        {
            get => _Chamber;
            set => SetProperty(ref _Chamber, value);
        }
        List<string> _LstUnit = Enum.GetNames(typeof(GaugeUnit)).ToList();
        public List<string> LstUnit
        {
            get => _LstUnit;
            set => SetProperty(ref _LstUnit, value);
        }
        string _EventLog = string.Empty;
        public string EventLog
        {
            get => _EventLog;
            set { SetProperty(ref _EventLog, value); }
        }
        public ICommand ShowHideBoosterCommand { get; set; }
        bool isShowBooster = true;
        public TestViewModel(ChamberViewModel chamberViewModels)
        {
            _Chamber = chamberViewModels;
            Chamber.LstPlasmaValve.Clear();
            Chamber.LstPlasmaValve.Add(new GasValve() { Name = "N2", IsOpen = true});
            Chamber.LstPlasmaValve.Add(new GasValve() { Name = "O2" });
            Chamber.LstPlasmaValve.Add(new GasValve() { Name = "Ar" });
            Chamber.LstPlasmaValve.Add(new GasValve() { Name = "H2" });
            Chamber.Gauge.CurrentValue = 5;
            ShowHideBoosterCommand = new RelayCommand(() => { isShowBooster = !isShowBooster; if (isShowBooster) { Chamber.PumpBlock.Booster.Visible = System.Windows.Visibility.Visible; } else { Chamber.PumpBlock.Booster.Visible = System.Windows.Visibility.Collapsed; } });
            //-------------------//

            Chamber.PumpBlock.Pump.ChangeStatusEvent += ChangeStatusEvent;
            Chamber.PumpBlock.Booster.ChangeStatusEvent += ChangeStatusEvent;
            Chamber.PumpBlock.Valve.ChangeStatusEvent += ChangeStatusEvent;
            Chamber.ExhaustValve.ChangeStatusEvent += ChangeStatusEvent;
            Chamber.LstPlasmaValve[0].ChangeStatusEvent += ChangeStatusEvent;
            Chamber.LstPlasmaValve[1].ChangeStatusEvent += ChangeStatusEvent;
            Chamber.LstPlasmaValve[2].ChangeStatusEvent += ChangeStatusEvent;
            Chamber.LstPlasmaValve[3].ChangeStatusEvent += ChangeStatusEvent;
        }

        
        private void ChangeStatusEvent(object arg1, bool arg2)
        {
            if(arg1 is Pump p) { EventLog += $"{p.Name}: {(arg2 ? "ON" : "OFF")}\n"; }
            if(arg1 is GasValve g) { EventLog += $"{g.Name}: {(arg2 ? "ON" : "OFF")}\n"; }
            
        }
    }
}
