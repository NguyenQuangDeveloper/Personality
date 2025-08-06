using AlarmConfig.Models.Common;
using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace AlarmConfig.ViewModels.Rights;

public class ShapeBlockViewModel : BaseViewModel
{
    private string _selectedShapeType;
    public string SelectedShapeType
    {
        get => _selectedShapeType;
        set => SetProperty(ref _selectedShapeType, value);
    }

    private ShapeTypeDraw _shapeTypeDraw;
    public ShapeTypeDraw ShapeTypeDraw
    {
        get => _shapeTypeDraw;
        set => SetProperty(ref _shapeTypeDraw, value);
    }

    private bool _selectAnimation;
    public bool SelectAnimation
    {
        get => _selectAnimation;
        set => SetProperty(ref _selectAnimation, value);
    }

    public ICommand SetShapeDrawCommand { get; set; }

    private const string _defaultShapeType = "Pointer";

    public ShapeBlockViewModel()
    {
        SetShapeDrawCommand = new RelayCommand<string>(SetShapeDrawExecute);

        SetShapeDrawExecute(_defaultShapeType);
    }

    private void SetShapeDrawExecute(string obj)
    {
        string[] type = obj.Split('_');

        SelectedShapeType = type[0];

        switch(obj)
        {
            case "Rectangle_Animation":
                ShapeTypeDraw = ShapeTypeDraw.Rectangle_Animation;
                SelectAnimation = true;
                break;

            case "Ellipse_Animation":
                ShapeTypeDraw = ShapeTypeDraw.Ellipse_Animation;
                SelectAnimation = true;
                break;

            case "Ellipse_Normal":
                ShapeTypeDraw = ShapeTypeDraw.Ellipse_Normal;
                SelectAnimation = false;
                break;

            case "Rectangle_Normal":
                ShapeTypeDraw = ShapeTypeDraw.Rectangle_Normal;
                SelectAnimation = false;
                break;

            case "Text_Normal":
                ShapeTypeDraw = ShapeTypeDraw.Text_Normal;
                SelectAnimation = false;
                break;

            case "Pointer":
                ShapeTypeDraw = ShapeTypeDraw.Pointer;
                SelectAnimation = false;
                break;
        }    
    }
}
