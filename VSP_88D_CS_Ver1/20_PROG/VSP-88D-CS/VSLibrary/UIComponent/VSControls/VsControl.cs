using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.UIComponent.VSControls.ViewRegion;
using VSLibrary.UIComponent.VsLogin;

namespace VSLibrary.UIComponent.VSControls;

public class VsControl : ContentControl
{
    private readonly Dictionary<string, UserControl> _views = new();
    private readonly PopupWindowManager _popupManager = new();
    private ViewSwitcher? _viewSwitcher;
    private Grid _containerGrid = null!;
    public VsControl()
    {
        ContentList = new ObservableCollection<string>();
        ContentTypeList = new ObservableCollection<string>();
    }

    public ObservableCollection<string> ContentList
    {
        get => (ObservableCollection<string>)GetValue(ContentListProperty);
        set => SetValue(ContentListProperty, value);
    }

    public static readonly DependencyProperty ContentListProperty =
        DependencyProperty.Register(nameof(ContentList), typeof(ObservableCollection<string>), typeof(VsControl),
            new PropertyMetadata(null, OnContentListChanged));

    public ObservableCollection<string> ContentTypeList
    {
        get => (ObservableCollection<string>)GetValue(ContentTypeListProperty);
        set => SetValue(ContentTypeListProperty, value);
    }

    public static readonly DependencyProperty ContentTypeListProperty =
        DependencyProperty.Register(nameof(ContentTypeList), typeof(ObservableCollection<string>), typeof(VsControl),
            new PropertyMetadata(null, OnContentListChanged));

    public string CurrentView
    {
        get => (string)GetValue(CurrentViewProperty);
        set => SetValue(CurrentViewProperty, value);
    }

    public static readonly DependencyProperty CurrentViewProperty =
        DependencyProperty.Register(nameof(CurrentView), typeof(string), typeof(VsControl),
            new PropertyMetadata(string.Empty, OnCurrentViewChanged));

    public bool UseSingletonView
    {
        get => (bool)GetValue(UseSingletonViewProperty);
        set => SetValue(UseSingletonViewProperty, value);
    }

    public static readonly DependencyProperty UseSingletonViewProperty =
        DependencyProperty.Register(nameof(UseSingletonView), typeof(bool), typeof(VsControl),
            new PropertyMetadata(true));

    private static void OnContentListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsControl ctrl)
            ctrl.TryInitViews();
    }

    private static void OnCurrentViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsControl ctrl && e.NewValue is string viewName)
        {
            ctrl.Dispatcher.BeginInvoke(new Action(() =>
            {
                ctrl._viewSwitcher?.Switch(viewName);
            }), DispatcherPriority.Background);
        }
    }


    private void TryInitViews()
    {
        if (ContentList == null || ContentTypeList == null) return;

        var names = ContentList.ToList();
        var types = ContentTypeList.ToList();

        if (names.Count != types.Count)
            throw new InvalidOperationException("ContentList와 ListType은 개수가 같아야 합니다.");

        InitViews(names, types);
    }

    /// <summary>
    /// View 이름 목록과 타입 목록을 기반으로 View와 ViewModel을 생성 및 바인딩합니다.
    /// - ViewModel은 Singleton 여부에 따라 다르게 Resolve됩니다.
    /// - RegionManager는 View 이름 기준으로 고정 등록됩니다.
    /// </summary>
    /// <param name="names">화면에 출력할 View 이름 목록입니다.</param>
    /// <param name="types">View에 매칭될 전체 Type 목록입니다. (전체 QualifiedName or FullName)</param>
    private void InitViews(List<string> names, List<string> types)
    {
        _views.Clear();

        // 컨테이너 초기화
        if (_containerGrid == null)
        {
            _containerGrid = new Grid();
            this.Content = _containerGrid;
        }
        else
        {
            _containerGrid.Children.Clear();
        }

        for (int i = 0; i < names.Count; i++)
        {
            var name = names[i];         // View 이름 (논리적 식별자)
            var typeName = types[i];     // View 타입 (FullName)

            // ✅ Popup 여부 판단
            bool isPopup = typeName.EndsWith("_Popup", StringComparison.OrdinalIgnoreCase);
            string actualTypeName = isPopup ? typeName[..^6] : typeName; // "_Popup" 잘라냄

            var result = ViewLoader.LoadViewWithViewModel(typeName, UseSingletonView);

            if (result.IsPopup)
            {
                _popupManager.CreatePopup(name, result.View, result.View.DataContext!);
            }
            else
            {
                _views[name] = result.View;

                if (UseSingletonView && VSContainer.Instance.RegionManager is RegionManager regionManager)
                {
                    regionManager.RegisterRegion(name, result.View.GetType());
                    regionManager.RegisterRegionControl(name, result.View);
                }

                _containerGrid.Children.Add(result.View);
                result.View.Visibility = UseSingletonView ? Visibility.Hidden : Visibility.Visible;
            }
        }

        _viewSwitcher = new ViewSwitcher(_popupManager.Windows, _views, UseSingletonView);
        _viewSwitcher.Switch(CurrentView);        
    }
}
