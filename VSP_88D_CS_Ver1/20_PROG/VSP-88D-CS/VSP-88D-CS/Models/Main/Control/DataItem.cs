using System.Windows.Media;

namespace VSP_88D_CS.Models.Main.Control
{
    public class DataItem
    {
        public string Loading { get; set; }
        public string Chamber { get; set; }
        public string Unloading { get; set; }
        public Brush LoadingBackground { get; set; } = Brushes.Gray;
        public Brush LoadingForeground { get; set; } = Brushes.White;
        public Brush ChamberBackground { get; set; } = Brushes.Gray;
        public Brush ChamberForeground { get; set; } = Brushes.White;
        public Brush UnloadingBackground { get; set; } = Brushes.Gray;
        public Brush UnloadingForeground { get; set; } = Brushes.White;
    }
}
