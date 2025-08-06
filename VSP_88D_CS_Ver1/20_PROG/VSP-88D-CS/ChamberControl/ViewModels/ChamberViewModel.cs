using System.Collections.ObjectModel;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl
{
    public class ChamberViewModel : ViewModelBase
    {
        ObservableCollection<GasValve> _LstPlasmaValve = new();
        public ObservableCollection<GasValve> LstPlasmaValve
        {
            get => _LstPlasmaValve;
            set => SetProperty(ref _LstPlasmaValve, value);
        }
        PumpBlock _PumpBlock = new();
        public PumpBlock PumpBlock
        {
            get => _PumpBlock;
            set => SetProperty(ref _PumpBlock, value);
        }
        GasValve _ExhaustValve = new() { Name = "ArH2" };
        public GasValve ExhaustValve
        {
            get => _ExhaustValve;
            set => SetProperty(ref _ExhaustValve, value);
        }
        Gauge _Gauge = new();
        public Gauge Gauge
        {
            get => _Gauge;
            set => SetProperty(ref _Gauge, value);
        }
        PlasmaCount _PlasmaCount = new();
        public PlasmaCount PlasmaCount
        {
            get => _PlasmaCount;
            set => SetProperty(ref _PlasmaCount, value);
        }
    }
}
