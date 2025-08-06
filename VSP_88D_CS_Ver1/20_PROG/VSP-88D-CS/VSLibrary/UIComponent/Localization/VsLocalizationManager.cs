using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VSLibrary.Common.Ini;

namespace VSLibrary.UIComponent.Localization;

/// <summary>
/// 지원되는 언어 종류입니다.
/// </summary>
public enum LanguageType
{
    English,
    Korean,
    Chinese,
    Vietnamese
}

/// <summary>
/// 다국어 문자열 로딩 및 자동 매칭을 담당하는 정적 클래스입니다.
/// </summary>
public static class VsLocalizationManager
{
    private static readonly Dictionary<LanguageType, Dictionary<string, Dictionary<string, string>>> _langDict= new();

    /// <summary>
    /// 현재 선택된 언어입니다. 기본값은 Korean입니다.
    /// </summary>
    public static LanguageType CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;         
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
        }
    }
    private static LanguageType _currentLanguage = LanguageType.Korean;

    /// <summary>
    /// 언어가 변경될 때 발생하는 이벤트입니다.
    /// </summary>
    public static event EventHandler? LanguageChanged;

    /// <summary>
    /// 현재 로드된 다국어 딕셔너리를 제공합니다. (읽기 전용)
    /// 구조: [언어][섹션][키] = 값
    /// </summary>
    public static IReadOnlyDictionary<LanguageType, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> Languages
        => _langDict.ToDictionary(
            lang => lang.Key,
            lang => (IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>)lang.Value.ToDictionary(
                section => section.Key,
                section => (IReadOnlyDictionary<string, string>)section.Value
            )
        );


    /// <summary>
    /// 언어를 로드하고 주어진 뷰에 자동 적용합니다.
    /// </summary>
    public static void Load(LanguageType language = LanguageType.English)
    {
        CurrentLanguage = language;
        foreach (LanguageType lang in Enum.GetValues(typeof(LanguageType)))
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LANG", $"UI_{lang.ToString()}.INI");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Localization file not found: {filePath}");

            var sectionDict = new Dictionary<string, Dictionary<string, string>>();
            var ini = new VsIniManagerProxy(); // 기존 VsIniManager를 읽기 전용으로 wrapping 추천
            ini.Load(filePath);

            foreach (var section in ini.GetSectionNames())
            {
                var keyDict = new Dictionary<string, string>();
                foreach (var key in ini.GetKeys(section))
                {
                    string? value = ini.GetValue(section, key);
                    if (!string.IsNullOrEmpty(value))
                        keyDict[key] = value;
                }

                sectionDict[section] = keyDict;
            }

            _langDict[lang] = sectionDict;
        }
    }

    /// <summary>
    /// 현재 언어 기준으로 문자열을 가져옵니다.
    /// </summary>
    public static string Get(string section, string key)
    {
        return Get(CurrentLanguage, section, key);
    }

    /// <summary>
    /// 다국어 문자열을 수동으로 가져옵니다.
    /// </summary>
    /// <param name="lang">언어 종류</param>
    /// <param name="section">INI 섹션 이름 (예: GUI, QUERY)</param>
    /// <param name="key">키 이름 (예: BtnSave)</param>
    /// <returns>로컬라이징 문자열 (없으면 !!section.key!! 형태 반환)</returns>
    public static string Get(LanguageType lang, string section, string key)
    {
        if (_langDict.TryGetValue(lang, out var sectionDict) &&
            sectionDict.TryGetValue(section, out var keyDict) &&
            keyDict.TryGetValue(key, out var value))
        {
            return value;
        }

        return null!; // fallback 표시
    }

    /// <summary>
    /// 특정 언어/섹션 내에서 번역된 값(Value)으로 원래의 키(Key)를 역으로 찾습니다.
    /// </summary>
    /// <param name="lang">언어</param>
    /// <param name="section">INI 섹션</param>
    /// <param name="value">번역된 문자열</param>
    /// <returns>매칭된 원본 키, 없으면 null</returns>
    public static string? GetKeyFromValue(LanguageType lang, string section, string value)
    {
        if (_langDict.TryGetValue(lang, out var sectionDict) &&
            sectionDict.TryGetValue(section, out var keyDict))
        {
            foreach (var pair in keyDict)
            {
                if (pair.Value == value)
                    return pair.Key;
            }
        }

        return null;
    }

}

public class StaticLocalizationProxy : DependencyObject, INotifyPropertyChanged
{
    public static readonly StaticLocalizationProxy Instance = new();

    public StaticLocalizationProxy()
    {
        VsLocalizationManager.LanguageChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(CurrentLanguage));
        };
    }

    public LanguageType CurrentLanguage
    {
        get => VsLocalizationManager.CurrentLanguage;
        set => VsLocalizationManager.CurrentLanguage = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}