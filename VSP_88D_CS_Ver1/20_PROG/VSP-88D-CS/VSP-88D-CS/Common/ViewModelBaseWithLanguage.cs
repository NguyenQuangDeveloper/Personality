using VSLibrary.Common.MVVM.ViewModels;
using VSFrameWork.Commons.Ignore;
using VSP_88D_CS.Models.Common.Database;

namespace VSP_88D_CS.Common;

public class ViewModelBaseWithLanguage : ViewModelBase
{
    private readonly LanguageRepository _languageRepository;
    private List<LanguageItem> _languages;

    /// <summary>
    /// ViewModelBaseWithLanguage 생성자
    /// </summary>
    /// <param name="languageRepository">언어 데이터를 제공하는 리포지토리</param>
    public ViewModelBaseWithLanguage(LanguageRepository languageRepository )
    {
        _languageRepository = languageRepository;
       ;
        // 언어 데이터 로드 및 초기화
        InitializeLanguages();
    }

    /// <summary>
    /// 언어 데이터를 초기화합니다.
    /// </summary>
    private void InitializeLanguages()
    {
        if (_languageRepository == null) return;
        _languages = _languageRepository.Data;

        if (_languages.Count > 0)
        {
            SetLanguage(_languages, "Kor");
        }
    }

    public void ChangeLanguage(string language)
    {           
        if (_languages != null && _languages.Any())
        {
            CurrentLanguage = language;
            // `MenuItemModel`의 `MenuText`를 `LanguageItem`에서 가져온 값으로 설정
            SetLanguage(_languages, language);
        }
    }
    private string _currentLanguage;

    /// <summary>
    /// 현재 선택된 언어 코드 (예: "Kor", "Eng", "Use1", "Use2").
    /// </summary>
    [IgnoreColumn]
    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged(nameof(CurrentLanguage));

                // 언어 변경 시 로직 실행
                ChangeLanguage(_currentLanguage);
            }
        }
    }
}
