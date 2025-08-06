using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VSLibrary.UIComponent.VSControls.ViewRegion;

/// <summary>
/// Popup View를 Window로 띄우고 관리하는 도우미 클래스입니다.
/// - View와 ViewModel을 바인딩하여 Window로 감쌉니다.
/// - 항상 TopMost이며, 기본 설정은 투명/작업표시줄 미표시입니다.
/// </summary>
public class PopupWindowManager
{
    private readonly Dictionary<string, Window> _popupWindows = new();

    /// <summary>
    /// 현재 등록된 팝업 윈도우 목록을 반환합니다.
    /// </summary>
    public IReadOnlyDictionary<string, Window> Windows => _popupWindows;

    /// <summary>
    /// 새 팝업 윈도우를 생성하고 ViewModel을 바인딩한 후 저장합니다.
    /// </summary>
    public void CreatePopup(string name, UserControl view, object viewModel)
    {
        var popup = new Window
        {
            Title = "",
            Content = view,
            Owner = Application.Current.MainWindow,
            Width = 340,
            Height = 400,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            AllowsTransparency = true,
            Background = Brushes.White,
            ShowInTaskbar = false,
            Topmost = true
        };

        view.DataContext = viewModel;

        popup.Loaded += (s, e) => popup.Hide(); // 또는 BeginInvoke
        popup.Show();       

        _popupWindows[name] = popup;
    }

    /// <summary>
    /// 모든 팝업을 닫고 내부 목록을 초기화합니다.
    /// </summary>
    public void Clear()
    {
        foreach (var popup in _popupWindows.Values)
        {
            if (popup.IsVisible)
                popup.Close();
        }

        _popupWindows.Clear();
    }

    /// <summary>
    /// 주어진 이름의 팝업을 보여줍니다.
    /// </summary>
    public void Show(string name)
    {
        if (_popupWindows.TryGetValue(name, out var popup))
        {
            if (!popup.IsVisible)
                popup.Show();

            popup.Activate();
        }
    }

    /// <summary>
    /// 주어진 이름을 제외한 모든 팝업을 숨깁니다.
    /// </summary>
    public void HideAllExcept(string name)
    {
        foreach (var kv in _popupWindows)
        {
            if (kv.Key != name && kv.Value.IsVisible)
                kv.Value.Hide();
        }
    }

    /// <summary>
    /// 모든 팝업을 닫습니다.
    /// </summary>
    public void CloseAll()
    {
        foreach (var popup in _popupWindows.Values)
            popup.Close();

        _popupWindows.Clear();
    }
}
