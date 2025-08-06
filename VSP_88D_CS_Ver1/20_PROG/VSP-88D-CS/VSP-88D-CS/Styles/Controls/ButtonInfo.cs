using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Styles.Controls
{
    public class ButtonInfo :ViewModelBase
    {
        public string Key { get; set; }
       
        private bool _isEnable;

        public bool IsEnable
        {
            get { return _isEnable; }
            set { SetProperty(ref _isEnable, value); } 
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        public string ImagePath {  get; set; }
    }
}
