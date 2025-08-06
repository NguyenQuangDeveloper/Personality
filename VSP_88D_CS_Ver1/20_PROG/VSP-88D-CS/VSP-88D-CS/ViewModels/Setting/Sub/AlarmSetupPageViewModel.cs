using System.Collections.ObjectModel;
using VSLibrary.Common.MVVM.Models;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;

namespace VSP_88D_CS.ViewModels.Setting.Sub;

public class AlarmSetupPageViewModel : ViewModelBase
{
    private IGlobalSystemOption _globalSystem;
    public ObservableCollection<ErrorItem> AlarmCodes { get; set; } = new();

    public AlarmSetupPageViewModel(IGlobalSystemOption globalSystem)
    {
        _globalSystem = globalSystem;
        AlarmCodes = new ((_globalSystem.Alarms));
    }
}
