using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AlarmConfig.ViewModels.Tops
{
    public class ImageToolViewModel : BaseViewModel
    {
        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                SetProperty(ref _height, value);
                _alarmViewModel.DrawingAreaVM.DisplayHeight = value;
                _originalHeight = value;
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                SetProperty(ref _width, value);
                _alarmViewModel.DrawingAreaVM.DisplayWidth = value;
                _originalWidth = value;
            }
        }

        public ICommand IncreaseWidthCommand { get; set; }
        public ICommand IncreaseHeightCommand { get; set; }
        public ICommand DecreaseWidthCommand { get; set; }
        public ICommand DecreaseHeightCommand { get; set; }

        private AlarmViewModel _alarmViewModel;
        private double _originalWidth;
        private double _originalHeight;

        public ImageToolViewModel(AlarmViewModel alarmViewModel)
        {
            _alarmViewModel = alarmViewModel;

            IncreaseWidthCommand = new RelayCommand(() => ChangeSize(deltaWidth: 20));
            IncreaseHeightCommand = new RelayCommand(() => ChangeSize(deltaHeight: 20));
            DecreaseWidthCommand = new RelayCommand(() => ChangeSize(deltaWidth: -20));
            DecreaseHeightCommand = new RelayCommand(() => ChangeSize(deltaHeight: -20));

            Initialize();
        }

        private void Initialize()
        {
            _originalWidth = _alarmViewModel.DrawingAreaVM.DisplayWidth;
            _originalHeight = _alarmViewModel.DrawingAreaVM.DisplayHeight;

            Height = _originalHeight;
            Width = _originalWidth;
        }

        private void ChangeSize(double deltaWidth = 0, double deltaHeight = 0)
        {
            double newWidth = Math.Max(100, _originalWidth + deltaWidth);
            double newHeight = Math.Max(100, _originalHeight + deltaHeight);

            double oldWidth = _originalWidth;
            double oldHeight = _originalHeight;

            _originalWidth = newWidth;
            _originalHeight = newHeight;

            double ratioX = newWidth / oldWidth;
            double ratioY = newHeight / oldHeight;

            foreach (var shape in _alarmViewModel.DrawingAreaVM.Shapes)
            {
                shape.X *= ratioX;
                shape.Y *= ratioY;
                shape.Width *= ratioX;
                shape.Height *= ratioY;
            }

            _alarmViewModel.DrawingAreaVM.DisplayWidth = newWidth;
            _alarmViewModel.DrawingAreaVM.DisplayHeight = newHeight;

            Width = _originalWidth;
            Height = _originalHeight;
        }
    }
}
