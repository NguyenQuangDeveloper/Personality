using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Recipe
{
    public class MotionParameter : ViewModelBase
    {
        public int Index { get; set; }
        public string Description { get; set; }
        private double _position;
        public double Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        // public ICommand SetCommand { get; set; } 
    }
}
