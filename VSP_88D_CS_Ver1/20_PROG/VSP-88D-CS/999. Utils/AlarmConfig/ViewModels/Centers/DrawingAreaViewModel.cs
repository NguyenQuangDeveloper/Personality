using AlarmConfig.Models.Common;
using AlarmConfig.ViewModels.Common;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VSLibrary.Common.MVVM.Core;
using Shape = AlarmConfig.Models.Shape;

namespace AlarmConfig.ViewModels.Centers;

public class DrawingAreaViewModel : BaseViewModel
{
    private ObservableCollection<Shape> _shapes = new();
    public ObservableCollection<Shape> Shapes
    {
        get => _shapes;
        set => SetProperty(ref _shapes, value);
    }

    private BitmapImage _imageAlarm;
    public BitmapImage ImageAlarm
    {
        get => _imageAlarm;
        set => SetProperty(ref _imageAlarm, value);
    }

    public ScaleTransform scaleTransform = new ScaleTransform(1, 1);
    public TranslateTransform translateTransform = new TranslateTransform(0, 0);

    private TransformGroup _combinedTransform;
    public Transform CombinedTransform => _combinedTransform ??= new TransformGroup
    {
        Children = { scaleTransform, translateTransform }
    };

    private double _displayWidth = 620;
    public double DisplayWidth
    {
        get => _displayWidth;
        set => SetProperty(ref _displayWidth, value);
    }

    private double _displayHeight = 270;
    public double DisplayHeight
    {
        get => _displayHeight;
        set => SetProperty(ref _displayHeight, value);
    }

    private double _selectRectX;
    public double SelectRectX { get => _selectRectX; set => SetProperty(ref _selectRectX, value); }

    private double _selectRectY;
    public double SelectRectY { get => _selectRectY; set => SetProperty(ref _selectRectY, value); }

    private double _selectRectWidth;
    public double SelectRectWidth { get => _selectRectWidth; set => SetProperty(ref _selectRectWidth, value); }

    private double _selectRectHeight;
    public double SelectRectHeight { get => _selectRectHeight; set => SetProperty(ref _selectRectHeight, value); }

    private bool _isSelecting;
    public bool IsSelecting { get => _isSelecting; set => SetProperty(ref _isSelecting, value); }

    private bool _isSelectingFromLeftToRight;
    public bool IsSelectingFromLeftToRight
    {
        get => _isSelectingFromLeftToRight;
        set => SetProperty(ref _isSelectingFromLeftToRight, value);
    }

    public ICommand StartDrawCommand { get; set; }
    public ICommand DrawMoveCommand { get; set; }
    public ICommand EndDrawCommand { get; set; }
    public ICommand SelectedShapeMouseMoveCommand { get; set; }
    public ICommand RegisterAnimationCommand { get; }
    public ICommand EditShapeCommand { get; }
    public ICommand FinishEditCommand { get; }
    public ICommand IncreaseShapeWidthCommand { get; set; }
    public ICommand DecreaseShapeWidthCommand { get; set; }
    public ICommand IncreaseShapeHeightCommand { get; set; }
    public ICommand DecreaseShapeHeightCommand { get; set; }

    private Shape _currentShape;
    private Shape _shapeMove;
    public Shape selectedShape;
    private ObservableCollection<Shape> _shapeSelecteds = new();
    private Point _startPoint;
    private bool _isMouseRightDown;
    private Point _startDragPoint;
    private bool _isDraggingImage = false;
    private KeyEventHandler? currentKeyDownHandler;

    private AlarmViewModel _alarmViewModel;

    public DrawingAreaViewModel(AlarmViewModel alarmViewModel)
    {
        _alarmViewModel = alarmViewModel;

        StartDrawCommand = new RelayCommand<MouseEventArgs>(StartDrawExecute);
        DrawMoveCommand = new RelayCommand<MouseEventArgs>(DrawMoveExecute);
        EndDrawCommand = new RelayCommand(EndDrawExecute);
        SelectedShapeMouseMoveCommand = new RelayCommand<Shape>(SelectedShapeMouseMoveExecute);
        EditShapeCommand = new RelayCommand<Shape>(EditShapeExecute);
        FinishEditCommand = new RelayCommand<object>(FinishEditExecute);
        IncreaseShapeWidthCommand = new RelayCommand<Shape>(shape => shape.Width += 5);
        DecreaseShapeWidthCommand = new RelayCommand<Shape>(shape => shape.Width = Math.Max(5, shape.Width - 5));
        IncreaseShapeHeightCommand = new RelayCommand<Shape>(shape => shape.Height += 5);
        DecreaseShapeHeightCommand = new RelayCommand<Shape>(shape => shape.Height = Math.Max(5, shape.Height - 5));
    }

    private void SelectedShapeMouseMoveExecute(Shape shape)
    {
        if (!_isMouseRightDown) return;
        if (selectedShape != null) return;
        foreach (var s in Shapes)
        {
            s.IsSelected = false;
        }

        if (shape != null)
        {
            shape.IsSelected = true;
        }
        selectedShape = shape;
    }

    private void GetShapeInROISelect(bool isEndDraw = false)
    {
        foreach (var shape in Shapes)
        {
            if (shape.X + shape.Width >= SelectRectX &&
                shape.X <= SelectRectX + SelectRectWidth &&
                shape.Y + shape.Height >= SelectRectY &&
                shape.Y <= SelectRectY + SelectRectHeight)
            {
                if (!IsSelectingFromLeftToRight && !isEndDraw)
                {
                    _shapeMove = shape;
                    shape.IsSelected = true;
                    return;
                }    
                else if(!IsSelectingFromLeftToRight && isEndDraw)
                {
                    shape.IsSelected = false;
                    continue;
                }    

                shape.IsSelected = true;
                _shapeSelecteds.Add(shape);
                EditShapeExecute(shape);
            }
            else
            {
                shape.IsSelected = false;
                shape.IsEditing = false;
            }
        }
        _isDraggingImage = false;

        _isMouseRightDown = false;
        _currentShape = null;
        selectedShape = null;
    }

    private void EndDrawExecute()
    {
        if(_alarmViewModel.ShapeBlockVM.ShapeTypeDraw == ShapeTypeDraw.Pointer)
        {
            IsSelecting = false;
            GetShapeInROISelect(true);
            IsSelectingFromLeftToRight = true;
            _shapeMove = null;
            return;
        }    

        _isDraggingImage = false;

        _isMouseRightDown = false;

        ClearSelectedShape();

        if (selectedShape != null)
        {
            selectedShape.IsSelected = false;
            selectedShape = null;
        }
        if (_currentShape == null) return;
        if (_currentShape.Height <= 10 || _currentShape.Width <= 10)
        {
            Shapes.Remove(_currentShape);
        }
        _currentShape = null;
    }

    private void DrawMoveExecute(MouseEventArgs e)
    {
        var canvas = e.Source as FrameworkElement;
        if (canvas == null) return;

        if (DraggingImage(e, canvas)) return;

        UpdatePosSelectShape(e);

        if (e.RightButton == MouseButtonState.Pressed || e.LeftButton == MouseButtonState.Released)
        {
            return;
        }

        Point currentPoint = e.GetPosition(canvas);

        var width = Math.Abs(currentPoint.X - _startPoint.X);
        var height = Math.Abs(currentPoint.Y - _startPoint.Y);

        var x = Math.Min(currentPoint.X, _startPoint.X);
        var y = Math.Min(currentPoint.Y, _startPoint.Y);

        if(IsSelecting && _shapeMove == null)
        {
            SelectRectX = x;
            SelectRectY = y;
            SelectRectWidth = width;
            SelectRectHeight = height;

            IsSelectingFromLeftToRight = currentPoint.X >= _startPoint.X;

            if(!IsSelectingFromLeftToRight)
            {
                GetShapeInROISelect();
                if(_shapeMove != null)
                {
                    IsSelecting = false;
                }    
            }    
        }

        if (_currentShape == null) return;

        _currentShape.Width = width;
        _currentShape.Height = height;

        _currentShape.X = x;
        _currentShape.Y = y;
    }

    private void StartDrawExecute(MouseEventArgs e)
    {
        var canvas = e.Source as FrameworkElement;
        if (canvas == null) return;

        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (scaleTransform.ScaleX > 1 || scaleTransform.ScaleY > 1)
                {
                    _isDraggingImage = true;
                    _startDragPoint = e.GetPosition(canvas);
                }

                return;
            }
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }
        _startPoint = e.GetPosition(canvas);

        ClearSelectedShape();

        if (_alarmViewModel.ShapeBlockVM.ShapeTypeDraw == ShapeTypeDraw.Pointer)
        {
            _shapeSelecteds.Clear();

            IsSelecting = true;
            

            SelectRectX = _startPoint.X;
            SelectRectY = _startPoint.Y;
            SelectRectWidth = 0;
            SelectRectHeight = 0;

            return;
        }    

        _currentShape = new Shape
        {
            X = _startPoint.X,
            Y = _startPoint.Y,
            ShapeType = _alarmViewModel.ShapeBlockVM.SelectedShapeType,
            Animation = _alarmViewModel.ShapeBlockVM.SelectAnimation,
            Color = _alarmViewModel.ColorToolVM.SelectedColor,
            ID = Shapes.Count.ToString(),
            IsSelected = true,
            StrokeThickness = _alarmViewModel.FontToolVM.SelectedLineWidth,
            Width = 0,
            Height = 0,
            Text = "Input content here",
            FontSize = _alarmViewModel.FontToolVM.SelectedFontSize,
            IsBold = _alarmViewModel.FontToolVM.IsBold,
            IsItalic = _alarmViewModel.FontToolVM.IsItalic
        };

        Shapes.Add(_currentShape);
    }

    private void ClearSelectedShape()
    {
        if (Shapes == null) return;

        foreach (var shape in Shapes)
        {
            shape.IsSelected = false;
            shape.IsEditing = false;
        }
    }

    private void UpdatePosSelectShape(MouseEventArgs e)
    {
        if (_shapeMove == null) return;
        var canvas = e.Source as FrameworkElement;
        Point currentPoint = e.GetPosition(canvas);
        _shapeMove.X = currentPoint.X;
        _shapeMove.Y = currentPoint.Y;
    }

    public void ExecuteAnimation(bool animationState)
    {
        if (Shapes == null) return;

        foreach (var shape in Shapes)
        {
            if (!animationState)
            {
                shape.IsAnimating = false;
                continue;
            }

            if (shape.Animation)
                shape.IsAnimating = true;
            else
                shape.IsAnimating = false;
        }
    }

    private void EditShapeExecute(Shape shape)
    {
        if (shape == null) return;
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) return;

        if (_alarmViewModel.ShapeBlockVM.ShapeTypeDraw != ShapeTypeDraw.Pointer)
        {
            foreach (var s in Shapes)
            {
                s.IsSelected = false;
                s.IsEditing = false;
            }
        }

        if(_alarmViewModel.ShapeBlockVM.ShapeTypeDraw == ShapeTypeDraw.Pointer && _shapeSelecteds.Count > 1)
        {
            foreach (var s in Shapes)
            {
                if(s == _shapeSelecteds[0])
                {
                    s.IsEditing = true;
                }    
            }
            shape.IsSelected = true;
        }    
        else
        {
            shape.IsEditing = true;
            shape.IsSelected = true;
        }    

        RegisterEventMouseLeftClick(shape);
    }

    private void RegisterEventMouseLeftClick(Shape shape)
    {
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            var window = Application.Current.MainWindow;
            if (window == null) return;

            if (currentKeyDownHandler != null)
                window.PreviewKeyDown -= currentKeyDownHandler;

            currentKeyDownHandler = (sender, e) =>
            {
                if (e.Key == Key.Delete)
                {
                    if(_alarmViewModel.ShapeBlockVM.ShapeTypeDraw == ShapeTypeDraw.Pointer)
                    {
                        foreach(var s in _shapeSelecteds)
                        {
                            Shapes.Remove(s);
                        }    
                    }
                    else
                    {
                        Shapes.Remove(shape);
                    }
                    shape.IsEditing = false;
                    e.Handled = true;

                    window.PreviewKeyDown -= currentKeyDownHandler;
                }
                else if (e.Key == Key.Enter)
                {
                    FinishEditExecute(shape);
                    e.Handled = true;
                    window.PreviewKeyDown -= currentKeyDownHandler;
                }
            };

            window.PreviewKeyDown += currentKeyDownHandler;
        }));
    }

    private void FinishEditExecute(object param)
    {
        if (param is Shape shape)
        {
            if (Keyboard.IsKeyDown(Key.Delete))
            {
                Shapes.Remove(shape);
                return;
            }

            if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&
                 Keyboard.IsKeyDown(Key.Enter))
            {
                return;
            }

            if (Keyboard.IsKeyDown(Key.Enter))
            {
                shape.IsEditing = false;
                shape.IsSelected = false;
                shape.IsBold = _alarmViewModel.FontToolVM.IsBold;
                shape.IsItalic = _alarmViewModel.FontToolVM.IsItalic;
                shape.FontSize = _alarmViewModel.FontToolVM.SelectedFontSize;
                shape.Color = _alarmViewModel.ColorToolVM.SelectedColor;
                shape.StrokeThickness = _alarmViewModel.FontToolVM.SelectedLineWidth;
                shape.Text = shape.Text.TrimEnd('\r', '\n');
            }
        }
    }

    private bool DraggingImage(MouseEventArgs e, FrameworkElement canvas)
    {
        if (!_isDraggingImage) return false;

        Point current = e.GetPosition(canvas);
        Vector delta = current - _startDragPoint;

        var newX = translateTransform.X + delta.X;
        var newY = translateTransform.Y + delta.Y;

        double canvasWidth = DisplayWidth;
        double canvasHeight = DisplayHeight;

        double scaleX = scaleTransform.ScaleX;
        double scaleY = scaleTransform.ScaleY;

        if (scaleX > 1)
        {
            double maxOffsetX = canvasWidth * (scaleX - 1);
            translateTransform.X = Math.Min(maxOffsetX, Math.Max(newX, -maxOffsetX));
        }
        else
        {
            translateTransform.X = newX;
        }

        if (scaleY > 1)
        {
            double maxOffsetY = canvasHeight * (scaleY - 1);
            translateTransform.Y = Math.Min(maxOffsetY, Math.Max(newY, -maxOffsetY));
        }
        else
        {
            translateTransform.Y = newY;
        }

        _startDragPoint = current;
        return true;
    }
}