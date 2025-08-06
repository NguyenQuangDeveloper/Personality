using AlarmConfig.Services.MultiLanguage;
using VSLibrary.Common.MVVM.ViewModels;

namespace AlarmConfig.ViewModels.Common;

public class BaseViewModel : ViewModelBase
{
    private Dictionary<string, string> _languageResources;
    public Dictionary<string, string> LanguageResources
    {
        get => _languageResources;
        set => SetProperty(ref _languageResources, value);
    }

    public BaseViewModel()
    {
        LanguageResources = LanguageViewModel.Instance.LanguageResources;
        LanguageViewModel.Instance.LanguageResourcesUpdated += Instance_LanguageResourcesUpdated;
    }

    private void Instance_LanguageResourcesUpdated()
    {
        LanguageResources = LanguageViewModel.Instance.LanguageResources;
    }
}
