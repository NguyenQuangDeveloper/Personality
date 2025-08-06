using VSLibrary.Common.MVVM.ViewModels;

namespace ChamberControl;

public class PlasmaCount : ViewModelBase
{
    int _Value = 0;
    public int Value
    {
        get => _Value;
        set => SetProperty(ref _Value, value);
    }
    string _Title = "Plasma Counts for 1 Hour";
    public string Title
    {
        get => _Title;
        set => SetProperty(ref _Title, value);
    }
}