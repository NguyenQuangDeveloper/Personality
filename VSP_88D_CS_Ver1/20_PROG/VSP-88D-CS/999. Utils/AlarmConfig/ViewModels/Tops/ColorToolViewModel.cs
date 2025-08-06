using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Windows.Media;

namespace AlarmConfig.ViewModels.Tops;

public class ColorToolViewModel : BaseViewModel
{
    private Brush _selectedColor;
    public Brush SelectedColor
    {
        get => _selectedColor;
        set => SetProperty(ref _selectedColor, value);
    }

    public ICommand SetColorDrawCommand { get; set; }

    public ColorToolViewModel()
    {
        SetColorDrawCommand = new RelayCommand<string>(SetColorDraw);

        Initial();
    }

    private void SetColorDraw(string color)
    {
        SelectedColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }

    private void Initial()
    {
        SelectedColor = Brushes.Black;
    }
}
