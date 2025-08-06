using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.Localization;

public static class VsLocalization
{
    public static readonly DependencyProperty LanguageTypeProperty =
        DependencyProperty.RegisterAttached(
            "LanguageType",
            typeof(LanguageType),
            typeof(VsLocalization),
            new PropertyMetadata(LanguageType.English, OnLanguageChanged));

    public static void SetLanguageType(DependencyObject obj, LanguageType value)
    {
        obj.SetValue(LanguageTypeProperty, value);
    }

    public static LanguageType GetLanguageType(DependencyObject obj)
    {
        return (LanguageType)obj.GetValue(LanguageTypeProperty);
    }

    public static readonly DependencyProperty LocalizationKeyProperty =
        DependencyProperty.RegisterAttached(
            "LocalizationKey",
            typeof(string),
            typeof(VsLocalization),
            new PropertyMetadata(null, OnLocalizationKeyChanged));

    public static void SetLocalizationKey(DependencyObject obj, string value)
    {
        obj.SetValue(LocalizationKeyProperty, value);
    }

    public static string GetLocalizationKey(DependencyObject obj)
    {
        return (string)obj.GetValue(LocalizationKeyProperty);
    }

    private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ApplyLocalization(d);
    }

    private static void OnLocalizationKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ApplyLocalization(d);
    }

    /// <summary>
    /// 다양한 WPF 컨트롤에 localizedText를 설정합니다.
    /// </summary>
    /// <param name="d">Control 객체 (object)</param>
    /// <param name="localizedText">로컬라이즈된 텍스트</param>
    private static void ApplyLocalization(DependencyObject d)
    {
        var key = GetLocalizationKey(d);
        var lang = GetLanguageType(d);

        if (string.IsNullOrWhiteSpace(key)) return;

        string? localizedText = TryFindLocalization(lang, key);
        if (string.IsNullOrWhiteSpace(localizedText)) return;

        switch (d)
        {
            // Menu, Tree, List 계열
            case MenuItem menu:
                menu.Header = localizedText;
                break;

            case TreeViewItem tree:
                tree.Header = localizedText;
                break;

            case ListViewItem list:
                list.Content = localizedText;
                break;

            case ComboBoxItem combo:
                combo.Content = localizedText;
                break;

            // Header 계열 먼저
            case GroupBox gb:
                gb.Header = localizedText;
                break;

            case TabItem tab:
                tab.Header = localizedText;
                break;

            case HeaderedContentControl hcc:
                hcc.Header = localizedText;
                break;

            case HeaderedItemsControl hic:
                hic.Header = localizedText;
                break;

            // Content 계열
            case Label lbl:
                lbl.Content = localizedText;
                break;

            case Button btn:
                btn.Content = localizedText;
                break;

            case CheckBox cb:
                cb.Content = localizedText;
                break;

            case RadioButton rb:
                rb.Content = localizedText;
                break;

            case ContentControl cc:
                cc.Content = localizedText;
                break;

            // Text 계열
            case TextBlock tb:
                tb.Text = localizedText;
                break;

            case TextBox tbx:
                tbx.Text = localizedText;
                break;

            // DataGrid 계열
            case DataGridTextColumn textCol:
                textCol.Header = localizedText;
                break;

            case DataGridCheckBoxColumn checkCol:
                checkCol.Header = localizedText;
                break;

            case DataGridComboBoxColumn comboCol:
                comboCol.Header = localizedText;
                break;

            case DataGridTemplateColumn templateCol:
                templateCol.Header = localizedText;
                break;

            // ToolTip이 따로 있는 경우
            case FrameworkElement fe when fe.ToolTip != null && fe.ToolTip is string:
                fe.ToolTip = localizedText;
                break;

            // 혹시 모를 fallback
            case FrameworkElement feFallback:
                // ex: Automation 속성 등 커스텀 처리 가능
                feFallback.ToolTip ??= localizedText;
                break;

            default:
                // 미처리 대상 로그 찍거나 무시
                Debug.WriteLine($"[Warn] 처리되지 않은 타입: {d.GetType().Name}");
                break;
        }
    }


    /// <summary>
    /// 언어/섹션 전체에서 해당 키를 탐색하여 첫 번째 매칭되는 값을 반환합니다.
    /// </summary>
    private static string? TryFindLocalization(LanguageType lang, string key)
    {
        if (VsLocalizationManager.Languages.TryGetValue(lang, out var sections))
        {
            foreach (var section in sections)
            {
                if (section.Value.TryGetValue(key, out var value))
                    return value;
            }
        }

        return null; // 못 찾으면 null
    }
}