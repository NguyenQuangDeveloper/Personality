using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AlarmConfig.ViewModels.Tops
{
    public class ZoomToolViewModel : BaseViewModel
    {
        public ICommand ZoomInCommand { get; }
        public ICommand ZoomOutCommand { get; }
        public ICommand ZoomResetCommand { get; }

        private double _zoomLevel = 1.0;
        private const double ZoomStep = 0.1;
        private const double MinZoom = 0.5;
        private const double MaxZoom = 3.0;

        private AlarmViewModel _alarmViewModel;

        public ZoomToolViewModel(AlarmViewModel alarmViewModel)
        {
            _alarmViewModel = alarmViewModel;

            ZoomInCommand = new RelayCommand(() => SetZoom(_zoomLevel + ZoomStep));
            ZoomOutCommand = new RelayCommand(() => SetZoom(_zoomLevel - ZoomStep));
            ZoomResetCommand = new RelayCommand(() =>
            {
                SetZoom(1.0);
                _alarmViewModel.DrawingAreaVM.translateTransform.X = 0;
                _alarmViewModel.DrawingAreaVM.translateTransform.Y = 0;
            });
        }

        private void SetZoom(double newZoom)
        {
            _zoomLevel = Math.Max(MinZoom, Math.Min(MaxZoom, newZoom));
            _alarmViewModel.DrawingAreaVM.scaleTransform.ScaleX = _zoomLevel;
            _alarmViewModel.DrawingAreaVM.scaleTransform.ScaleY = _zoomLevel;
        }
    }
}
