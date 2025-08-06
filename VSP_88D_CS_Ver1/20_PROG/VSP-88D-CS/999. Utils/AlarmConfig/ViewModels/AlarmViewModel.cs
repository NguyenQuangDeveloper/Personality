using AlarmConfig.ViewModels.Bottoms;
using AlarmConfig.ViewModels.Centers;
using AlarmConfig.ViewModels.Common;
using AlarmConfig.ViewModels.Lefts;
using AlarmConfig.ViewModels.Rights;
using AlarmConfig.ViewModels.Tops;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Models;

namespace AlarmConfig.ViewModels;

public class AlarmViewModel : BaseViewModel
{
    public AlarmInfoViewModel AlarmInfoVM { get; set; }
    public DrawingAreaViewModel DrawingAreaVM { get; set; }
    public AlarmCodeViewModel AlarmCodeVM { get; set; }
    public ShapeBlockViewModel ShapeBlockVM { get; set; }
    public ColorToolViewModel ColorToolVM { get; set; }
    public FileToolViewModel FileToolVM { get; set; }
    public ZoomToolViewModel ZoomToolVM { get; set; }
    public ImageToolViewModel ImageToolVM { get; set; }
    public FontToolViewModel FontToolVM { get; set; }
    public ObservableCollection<ErrorItem> AlarmCodes { get; set; }

    public ICommand KeyPressedCommand { get; set; }

    public AlarmViewModel()
    {
        Initialize();
        KeyPressedCommand = new RelayCommand<KeyEventArgs>(KeyPressed);
    }
    
    private void Initialize()
    {
        AlarmInfoVM = new(this);
        DrawingAreaVM = new(this);
        AlarmCodeVM = new(this);
        ShapeBlockVM = new();
        ColorToolVM = new();
        FileToolVM = new(this);
        ZoomToolVM = new(this);
        ImageToolVM = new(this);
        FontToolVM = new(this);
    }

    private void KeyPressed(KeyEventArgs e)
    {
        if (e == null) return;
        if (e.Key == Key.Delete && DrawingAreaVM.selectedShape != null)
        {
            DrawingAreaVM.Shapes.Remove(DrawingAreaVM.selectedShape);
            DrawingAreaVM.selectedShape = null;
        }
    }
}
