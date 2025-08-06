using VSLibrary.Common.MVVM.ViewModels;

namespace AlarmConfig.Models;

public class Button : ViewModelBase
{
    private string _content;
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    private string _message;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private string _solution;
    public string Solution
    {
        get => _solution;
        set => SetProperty(ref _solution, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
}
