using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.VSControls.ViewRegion;

/// <summary>
/// View 및 팝업 전환을 처리하는 전용 헬퍼 클래스입니다.
/// - SingletonView 모드에서만 동작하며,
/// - 일반 View 또는 Popup을 활성화합니다.
/// </summary>
public class ViewSwitcher
{
    private readonly IReadOnlyDictionary<string, Window> _popupWindows;
    private readonly Dictionary<string, UserControl> _views;
    private readonly bool _useSingleton;

    public ViewSwitcher(
        IReadOnlyDictionary<string, Window> popupWindows,
        Dictionary<string, UserControl> views,
        bool useSingletonView)
    {
        _popupWindows = popupWindows;
        _views = views;
        _useSingleton = useSingletonView;
    }

    /// <summary>
    /// 지정된 View 이름을 기준으로 화면 전환을 수행합니다.
    /// </summary>
    public void Switch(string viewName)
    {
        if (!_useSingleton) return;

        // Popup이면 팝업 열기
        if (_popupWindows.TryGetValue(viewName, out var popup))
        {
            if (!popup.IsVisible)
                popup.Show();

            popup.Activate();
        }
        else
        {
            // 모든 팝업 닫기
            foreach (var dlg in _popupWindows.Values)
            {
                if (dlg.IsVisible)
                    dlg.Hide();
            }

            // 일반 View 전환
            foreach (var kv in _views)
            {
                kv.Value.Visibility = kv.Key == viewName
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }
        }
    }
}