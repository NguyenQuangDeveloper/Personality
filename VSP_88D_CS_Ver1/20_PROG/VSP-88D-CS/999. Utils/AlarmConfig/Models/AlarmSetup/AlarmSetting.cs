using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AlarmConfig.Models.AlarmSetup
{
    public class AlarmSetting
    {
        public Dictionary<string, AlarmControl> Alarms { get; set; }
        public AlarmSetting()
        {
            Alarms = new Dictionary<string, AlarmControl>();
        }
    }
    public class AlarmControl
    {
        public List<Shape> ShapeMDs { get; set; }
        public ImageMD ImageMD { get; set; }
        public AlarmControl()
        {
            ShapeMDs = new List<Shape>();
            ImageMD = new ImageMD();
        }
    }
    public class ShapeMD
    {
        public ShapeMD()
        {
        }
        public ShapeMD(double x, double y, double width, double height, double strokeThickness,
            string shapeType, string iD, Brush color, bool isAnimating, bool isSelected, bool animation, string text,
            double fontSize, bool isBold, bool isItalic)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            StrokeThickness = strokeThickness;
            ShapeType = shapeType;
            ID = iD;
            Color = color;
            IsAnimating = isAnimating;
            IsSelected = isSelected;
            Animation = animation;
            Text = text;
            FontSize = fontSize;
            IsBold = isBold;
            IsItalic = isItalic;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double StrokeThickness{ get; set; }
        public string ShapeType { get; set; }
        public string ID { get; set; }
        public Brush Color { get; set; }
        public bool IsAnimating { get; set; }
        public bool IsSelected { get; set; }
        public bool Animation {  get; set; }
        public string Text { get; set; }
        public bool IsEditing {  get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public double FontSize { get; set; }
    }
    public class ImageMD
    {
        public string Path { get; set; }
        public double HeightImage { get; set; } = 350;
        public double WidthImage { get; set; } = 620;
    }
}
