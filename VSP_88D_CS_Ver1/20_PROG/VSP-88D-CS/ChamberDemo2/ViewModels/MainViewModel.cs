using ChamberControl;
using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberDemo2.ViewModels;

public class MainViewModel : ViewModelBase
{
    ChamberViewModel _ChamberViewModels;
    public ChamberViewModel ChamberViewModels
    {
        get => _ChamberViewModels;
        set => SetProperty(ref _ChamberViewModels, value);
    }   
    public MainViewModel(ChamberViewModel chamberViewModels)
    {
        _ChamberViewModels = chamberViewModels;       
    }
}
