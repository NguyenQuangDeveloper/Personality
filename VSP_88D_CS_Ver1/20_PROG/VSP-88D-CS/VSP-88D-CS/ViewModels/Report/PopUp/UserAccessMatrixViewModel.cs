using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;

namespace VSP_88D_CS.ViewModels.Report.PopUp
{
    public class UserAccessMatrixViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        public ICommand ClosingCommand { get; }

        public UserAccessMatrixViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            ClosingCommand = new RelayCommand(OnClosing);
          //  InitData();

        }

        private void OnClosing()
        {
            Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
    }

}
