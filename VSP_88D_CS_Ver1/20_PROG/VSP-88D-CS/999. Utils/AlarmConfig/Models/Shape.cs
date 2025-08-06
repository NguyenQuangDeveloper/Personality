using AlarmConfig.Models.AlarmSetup;
using System.Windows.Media;
using VSLibrary.Common.MVVM.ViewModels;

namespace AlarmConfig.Models
{
    public class Shape : ViewModelBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _id;
        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private Brush _color;
        public Brush Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private string _shapeType;
        public string ShapeType
        {
            get => _shapeType;
            set => SetProperty(ref _shapeType, value);
        }

        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private double _strokeThickness;
        public double StrokeThickness
        {
            get => _strokeThickness;
            set => SetProperty(ref _strokeThickness, value);
        }

        private bool _isAnimating;
        public bool IsAnimating
        {
            get => _isAnimating;
            set => SetProperty(ref _isAnimating, value);
        }

        private bool _animation;
        public bool Animation
        {
            get => _animation;
            set => SetProperty(ref _animation, value);
        }

        private string _text = "Input";
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private double _fontSize = 16;
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private bool _isBold = true;
        public bool IsBold
        {
            get => _isBold;
            set => SetProperty(ref _isBold, value);
        }

        private bool _isItalic = true;
        public bool IsItalic
        {
            get => _isItalic;
            set => SetProperty(ref _isItalic, value);
        }

        public Shape()
        {

        }
        public Shape(ShapeMD shapeMD)
        {
            IsSelected = shapeMD.IsSelected;
            ID = shapeMD.ID;
            Color = shapeMD.Color;
            ShapeType = shapeMD.ShapeType;
            IsAnimating = shapeMD.IsAnimating;
            Animation = shapeMD.Animation;
            X = shapeMD.X;
            Y = shapeMD.Y;
            Width = shapeMD.Width;
            Height = shapeMD.Height;
            StrokeThickness = shapeMD.StrokeThickness;
            Text = shapeMD.Text;
            IsEditing = shapeMD.IsEditing;
            FontSize = shapeMD.FontSize;
            IsBold = shapeMD.IsBold;
        }
        public Shape(Shape shapeMD)
        {
            IsSelected = shapeMD.IsSelected;
            ID = shapeMD.ID;
            Color = shapeMD.Color;
            ShapeType = shapeMD.ShapeType;
            IsAnimating = shapeMD.IsAnimating;
            Animation = shapeMD.Animation;
            X = shapeMD.X;
            Y = shapeMD.Y;
            Width = shapeMD.Width;
            Height = shapeMD.Height;
            StrokeThickness = shapeMD.StrokeThickness;
            Text = shapeMD.Text;
            IsEditing = shapeMD.IsEditing;
            FontSize = shapeMD.FontSize;
            IsBold = shapeMD.IsBold;
        }
    }
}
