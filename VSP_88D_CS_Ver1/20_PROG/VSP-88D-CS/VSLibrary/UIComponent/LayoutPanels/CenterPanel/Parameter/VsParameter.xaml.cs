using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VSLibrary.Common.Ini;
using VSLibrary.UIComponent.Localization;

namespace VSLibrary.UIComponent.LayoutPanels.CenterPanel.Parameter;

/// <summary>
/// VsParameter.xaml에 대한 상호 작용 논리
/// </summary>
public partial class VsParameter : UserControl
{
    public VsParameter()
    {
        InitializeComponent();

        if (!string.IsNullOrEmpty(SavePath))
        {
            VsIniManager.Initialize(SavePath);
        }
        else
        {
            VsIniManager.Initialize(VsSaveService.CurrentConfigPath);
        }
        if(ParameterItems == null)
        {
            ParameterItems = new ObservableCollection<VsParameterData>();
        }
        ParameterItems.Clear();
        // UI 로딩시 섹션 목록을 자동으로 얻기
        var allSections = VsIniManager.GetSectionNames();
        foreach (var section in allSections)
        {
            LoadParameterList(section);
        }

        VsLocalizationManager.LanguageChanged += (_, _) =>
        {
            foreach (var item in ParameterItems)
            {
                item.UpdateLocalization();
            }
        };
    }

    public ObservableCollection<VsParameterData> ParameterItems
    {
        get { return (ObservableCollection<VsParameterData>)GetValue(ParameterItemsProperty); }
        set { SetValue(ParameterItemsProperty, value); }
    }

    public static readonly DependencyProperty ParameterItemsProperty =
        DependencyProperty.Register(
            nameof(ParameterItems),
            typeof(ObservableCollection<VsParameterData>),
            typeof(VsParameter),
            new PropertyMetadata(new ObservableCollection<VsParameterData>()));

    /// <summary>
    /// INI 저장 경로를 외부에서 주입받는 속성입니다.
    /// </summary>
    public static readonly DependencyProperty SavePathProperty =
        DependencyProperty.Register(
            nameof(SavePath),
            typeof(string),
            typeof(VsParameter),
            new PropertyMetadata(default(string)));

    /// <summary>
    /// INI 저장 경로입니다.
    /// </summary>
    public string SavePath
    {
        get => (string)GetValue(SavePathProperty);
        set => SetValue(SavePathProperty, value);
    }

    private void VsButton_SaveClick(object sender, RoutedEventArgs e)
    {
        UpdateParameterItemsFromUI();

        if (!string.IsNullOrEmpty(SavePath))
        {
            VsIniManager.Save( SavePath );
        }
       else
        {
            VsIniManager.Save(VsSaveService.CurrentConfigPath);
        }
    }

    /// <summary>
    /// UI의 파라미터 값을 모델(ParameterItems)에 반영하는 함수
    /// (ViewModel 내 바인딩/수정 값 → 실제 저장용 데이터로 복사)
    /// </summary>
    private void UpdateParameterItemsFromUI()
    {
        // 예시: 파라미터 바인딩이 ObservableCollection<ParameterItem>인 경우
        // 각 ParameterItem의 Value를 실제 모델/저장소에 반영
        foreach (var item in ParameterItems)
        {
            // 필요시 VsIniManager.Set(item.Key, item.Value) 등으로 갱신
            if (item.Type != ParameterType.TitleBar)
            {
                var optionString = item.OptionItems != null ? string.Join("|", item.OptionItems) : "";
                var sb = new StringBuilder();
                sb.Append(item.Value ?? "").Append(",");
                sb.Append(item.Min ?? "").Append(",");
                sb.Append(item.Max ?? "").Append(",");
                sb.Append(item.DefaultValue ?? "").Append(",");
                sb.Append(item.Type.ToString()).Append(",");
                sb.Append(optionString).Append(",");
                sb.Append(item.DisplayText ?? "").Append(",");
                sb.Append(item.ToolTip ?? "");
            
                VsIniManager.Set( item.Section!, item.Key!, sb.ToString());
            }
        }
    }

    private void VsButton_DefaultClick(object sender, RoutedEventArgs e)
    {
        VsIniManager.Load(VsSaveService.DefaultConfigPath);

           ParameterItems.Clear();
        // UI 로딩시 섹션 목록을 자동으로 얻기
        var allSections = VsIniManager.GetSectionNames();
        foreach (var section in allSections)
        {
            LoadParameterList(section);
        }
    }

    /// <summary>
    /// INI 파일에서 파라미터 데이터를 읽어와 리스트로 변환합니다.
    /// </summary>
    /// <param name="section">INI 섹션명</param>
    /// <returns>VsParameterData 리스트</returns>
    public void LoadParameterList(string section)
    {
        ParameterItems?.Add(new VsParameterData
        {
            Section = section,
            Key = section + "_TitleBar",
            Value = "0",
            Min = "0",
            Max = "0",
            DefaultValue = "0",
            Type = ParameterType.TitleBar,
            OptionItems = null!,
            DisplayText = VsLocalizationManager.Get(VsLocalizationManager.CurrentLanguage, section, section + "_TitleBar") 
                            ?? section + " TitleBar",
            ToolTip = VsLocalizationManager.Get(VsLocalizationManager.CurrentLanguage, section, section + "_TitleBar_Tooltip")
                            ?? ""
        });

        var keys = VsIniManager.GetKeys(section); // ← 실제 Key 목록 반환

        foreach (var key in keys)
        {
            var raw = VsIniManager.Get(section, key);
            if (raw == null) continue;
            var vals = raw.Split(',').Select(x => x.Trim()).ToArray();
            
            ParameterItems?.Add(new VsParameterData
            {
                Section = section,
                Key = key,
                Value = vals.ElementAtOrDefault(0) ?? "",
                Min = vals.ElementAtOrDefault(1) ?? "",
                Max = vals.ElementAtOrDefault(2) ?? "",
                DefaultValue = vals.ElementAtOrDefault(3) ?? "",
                Type = Enum.TryParse(vals.ElementAtOrDefault(4), true, out ParameterType t) ? t : ParameterType.Text,
                OptionItems = VsLocalizationManager.Get(
                                VsLocalizationManager.CurrentLanguage, section, key + "_OptionItem")
                                ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                ?? (vals.ElementAtOrDefault(5) ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries),
                DisplayText = VsLocalizationManager.Get( VsLocalizationManager.CurrentLanguage, section, key) 
                                ?? vals.ElementAtOrDefault(6) ?? key,
                ToolTip = VsLocalizationManager.Get(VsLocalizationManager.CurrentLanguage, section, key + "_Tooltip") 
                                ?? vals.ElementAtOrDefault(7) ?? ""
            });
        }
    }
}

/// <summary>
/// C# 진입장벽을 낮추기 위한 서비스 클래스입니다.
/// </summary>
public static class VsSaveService
{
    public static string CurrentConfigPath { get; set; } = "";
    public static string DefaultConfigPath { get; set; } = "";
}

/// <summary>
/// For initial simplicity, this class was implemented in a single file.
/// Maintenance may become difficult as complexity increases.
/// If modifications are needed, consider extracting an interface and refactoring into separate modules.
/// </summary>
public partial class VsParameterData : ObservableObject
{
    /// <summary>
    /// 내부적으로 처리하기 때문에 필요한 곳에서만 사용 하면 됩니다.
    /// </summary>
    public static event EventHandler<string>? LanguageChanged;

    public static event EventHandler<ComboChangedEventArgs>? ComboChanged;
    public static event EventHandler<CheckChangedEventArgs>? CheckChanged;
    /// <summary>
    /// 섹션명 (INI/구성 파일에서 그룹 구분용)
    /// </summary>
    [ObservableProperty]
    private string ?_section;

    /// <summary>키 이름</summary>
    [ObservableProperty]
    private string ?_key;

    /// <summary>현재 값</summary>
    [ObservableProperty]
    private string ?_value;

    /// <summary>최소값</summary>
    [ObservableProperty]
    private string ?_min;

    /// <summary>최대값</summary>
    [ObservableProperty]
    private string? _max;

    /// <summary>기본값</summary>
    [ObservableProperty]
    private string ?_defaultValue;

    /// <summary>파라미터 타입</summary>
    [ObservableProperty]
    private ParameterType _type;

    /// <summary>콤보박스 아이템 (|로 구분)</summary>
    [ObservableProperty]
    private string[] ?_optionItems;

    /// <summary>화면 표시 텍스트</summary>
    [ObservableProperty]
    private string ?_displayText = string.Empty!;

    /// <summary>툴팁</summary>
    [ObservableProperty]
    private string? _toolTip = string.Empty!;

    /// <summary>
    /// DataGridRow 표시 여부 (bool → Converter 또는 XAML에서 Visibility 변환)
    /// </summary>
    [ObservableProperty]
    private bool _isEditable = true;

    /// <summary>
    /// DataGridRow 표시 여부 (bool → Converter 또는 XAML에서 Visibility 변환)
    /// </summary>
    [ObservableProperty]
    private bool _isVisible = true;

    public string CurrentCheckText =>
        (Value == "1" || Value?.ToLower() == "true")
            ? OptionItems?.FirstOrDefault() ?? ""
            : OptionItems?.LastOrDefault() ?? "";

    public string AdditionalText =>
        (Value == "1" || Value?.ToLower() == "true")
            ? OptionItems?.LastOrDefault() ?? ""
            : OptionItems?.FirstOrDefault() ?? "";

    partial void OnValueChanged(string ?oldValue, string? newValue)
    {
        // ComboBox일 때 CurrentCheckText와 AdditionalText 갱신
        switch(_type)
        {
            case ParameterType.ComboBox:
                // 외부로 이벤트 발생시키기 (언어 변경이나 값 변경 시)
                if (_key == "LANG")
                {
                    LanguageChanged?.Invoke(this, newValue ?? ""); // newValue는 변경된 값 (언어 등)           
                }
                else
                {
                    ComboChanged?.Invoke(this, new ComboChangedEventArgs(_key!, newValue ?? ""));
                }
                break;

            case ParameterType.CheckBox:
                bool  value = (newValue == "1" || newValue?.ToLower() == "true");
                CheckChanged?.Invoke(this, new CheckChangedEventArgs(_key!, value));
                break;
        }       

        OnPropertyChanged(nameof(CurrentCheckText));
        OnPropertyChanged(nameof(AdditionalText));
    }

    public void UpdateLocalization()
    {
        if (string.IsNullOrWhiteSpace(Key) || string.IsNullOrWhiteSpace(Section))
            return;

        DisplayText = VsLocalizationManager.Get(
            VsLocalizationManager.CurrentLanguage, Section, Key) ?? DisplayText;

        ToolTip = VsLocalizationManager.Get(
            VsLocalizationManager.CurrentLanguage, Section, Key + "_Tooltip") ?? ToolTip;

        var oldValue = Value;

        OptionItems = VsLocalizationManager.Get(
            VsLocalizationManager.CurrentLanguage, Section, Key + "_OptionItem")
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            ?? OptionItems;

        // 콤보박스 항목도 언어가 바뀌면 갱신 필요
        switch(Type)
        {
            case ParameterType.ComboBox:
                if(OptionItems?.Any() == true)
                {
                    Value = oldValue;
                    OnPropertyChanged(nameof(Value));
                    // ComboBox 항목 갱신                 
                    OnPropertyChanged(nameof(OptionItems));  // Option          

                    OnPropertyChanged(nameof(CurrentCheckText));
                    OnPropertyChanged(nameof(AdditionalText));
                }
                break;

            case ParameterType.CheckBox:
                OnPropertyChanged(nameof(OptionItems));  // Option    
                OnPropertyChanged(nameof(CurrentCheckText));
                OnPropertyChanged(nameof(AdditionalText));
                break;

            default:
                break;
        }     
    }


    /// <summary>
    /// 파일/디렉토리 선택 다이얼로그 열기
    /// </summary>
    [RelayCommand]
    private void Browse()
    {
        if (Type == ParameterType.FilePath)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            if (dlg.ShowDialog() == true)
                Value = dlg.FileName;
        }
        else if (Type == ParameterType.DirectoryPath)
        {
            var dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                Value = dlg.FileName;
        }
    }
}

/// <summary>
/// 파라미터의 유형을 정의하는 열거형입니다.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// 숫자 형식의 파라미터.
    /// </summary>
    Number,

    /// <summary>
    /// ComboBox 형식의 파라미터.
    /// </summary>
    ComboBox,

    /// <summary>
    /// CheckBox 형식의 파라미터.
    /// </summary>
    CheckBox,

    /// <summary>
    /// 텍스트 형식의 파라미터.
    /// </summary>
    Text,

    /// <summary>
    /// 파일 경로를 선택하는 파라미터.
    /// </summary>
    FilePath,

    /// <summary>
    /// 디렉토리 경로를 선택하는 파라미터.
    /// </summary>
    DirectoryPath,

    /// <summary>
    /// 타이틀 바 형식의 파라미터. UI의 제목 표시줄을 의미할 수 있음.
    /// </summary>
    TitleBar
}

public class ParameterTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// 숫자 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate NumberTemplate { get; set; } = null!;

    /// <summary>
    /// 콤보 박스 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate ComboBoxTemplate { get; set; } = null!;

    /// <summary>
    /// 체크 박스 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate CheckBoxTemplate { get; set; } = null!;

    /// <summary>
    /// 텍스트 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate TextTemplate { get; set; } = null!;

    /// <summary>
    /// 파일 경로 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate FilePathTemplate { get; set; } = null!;

    /// <summary>
    /// 디렉토리 경로 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate DirectoryPathTemplate { get; set; } = null!;

    /// <summary>
    /// 타이틀 바 타입 파라미터에 대한 템플릿입니다.
    /// </summary>
    public DataTemplate TitleBarTemplate { get; set; } = null!;

    /// <summary>
    /// 읽기 전용 파라미터에 대한 기본 템플릿입니다.
    /// </summary>
    public DataTemplate ReadOnlyTemplate { get; set; } = null!;

    /// <summary>
    /// 파라미터 타입에 따라 적절한 템플릿을 선택합니다.
    /// </summary>
    /// <param name="item">현재 파라미터 항목입니다.</param>
    /// <param name="container">템플릿이 적용될 컨테이너 객체입니다.</param>
    /// <returns>해당 파라미터 타입에 맞는 <see cref="DataTemplate"/>을 반환합니다.</returns>
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is VsParameterData parameterItem)
        {
            switch (parameterItem.Type)
            {
                case ParameterType.Number:
                    return NumberTemplate;
                case ParameterType.ComboBox:
                    return ComboBoxTemplate;
                case ParameterType.CheckBox:
                    return CheckBoxTemplate;
                case ParameterType.Text:
                    return TextTemplate;
                case ParameterType.FilePath:
                    return FilePathTemplate;
                case ParameterType.DirectoryPath:
                    return DirectoryPathTemplate;
                case ParameterType.TitleBar:
                    return TitleBarTemplate;
                default:
                    return ReadOnlyTemplate;
            }
        }
        return base.SelectTemplate(item, container);
    }
}

public class ComboChangedEventArgs : EventArgs
{
    public string Key { get; }
    public string Value { get; }

    public ComboChangedEventArgs(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

public class CheckChangedEventArgs : EventArgs
{
    public string Key { get; }
    public bool IsChecked { get; }

    public CheckChangedEventArgs(string key, bool isChecked)
    {
        Key = key;
        IsChecked = isChecked;
    }
}
