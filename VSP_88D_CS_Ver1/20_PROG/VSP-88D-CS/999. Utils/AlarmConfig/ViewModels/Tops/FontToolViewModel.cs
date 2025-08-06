using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AlarmConfig.ViewModels.Tops
{
    public class FontToolViewModel : BaseViewModel
    {
        private ObservableCollection<double> _lineWidths = new();
        public ObservableCollection<double> LineWidths
        {
            get => _lineWidths;
            set => SetProperty(ref _lineWidths, value);
        }

        private double _selectedLineWidth;
        public double SelectedLineWidth
        {
            get => _selectedLineWidth;
            set => SetProperty(ref _selectedLineWidth, value);
        }

        private ObservableCollection<double> _fontSizes = new();
        public ObservableCollection<double> FontSizes
        {
            get => _fontSizes;
            set => SetProperty(ref _fontSizes, value);
        }

        private double _selectedFontSize;
        public double SelectedFontSize
        {
            get => _selectedFontSize;
            set => SetProperty(ref _selectedFontSize, value);
        }

        private bool _isBold;
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        private bool _isItalic;
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }


        public ICommand SelectedFontWeightCommand { get; set; }
        public ICommand SelectedFontStyleCommand { get; set; }

        private AlarmViewModel _alarmViewModel;

        public FontToolViewModel(AlarmViewModel alarmViewModel)
        {
            _alarmViewModel = alarmViewModel;
            CreateDefault();

            SelectedFontWeightCommand = new RelayCommand(SelectedFontWeightExecute);
            SelectedFontStyleCommand = new RelayCommand(SelectedFontStyleExecute);
        }

        private void CreateDefault()
        {
            CreateDefaultLineWidth();
            CreateDefaultFontSize();
        }

        private void CreateDefaultLineWidth()
        {
            for (int i = 1; i <= 10; i++)
            {
                this.LineWidths.Add(i);
            }
            this.SelectedLineWidth = 2;
        }

        private void CreateDefaultFontSize()
        {
            for (int i = 8; i <= 50; i++)
            {
                this.FontSizes.Add(i);
            }
            SelectedFontSize = 16;
        }

        private void SelectedFontWeightExecute()
        {
            IsBold = !IsBold;
        }

        private void SelectedFontStyleExecute()
        {
            IsItalic = !IsItalic;
        }
    }
}
