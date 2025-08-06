using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using VSLibrary.Common.MVVM.Core;

namespace VSLibrary.UIComponent.VSControls.ViewRegion;

/// <summary>
/// View 및 ViewModel을 타입 이름으로부터 로드하는 도우미 클래스입니다.
/// - View 인스턴스를 생성하고
/// - ViewModel을 DI로 Resolve하며
/// - DataContext에 바인딩까지 수행합니다.
/// </summary>
public static class ViewLoader
{
    public class LoadedViewInfo
    {
        /// <summary>
        /// 생성된 View 인스턴스입니다.
        /// </summary>
        public UserControl View { get; set; } = null!;

        /// <summary>
        /// 연결된 ViewModel 타입입니다. 없을 경우 null입니다.
        /// </summary>
        public Type? ViewModelType { get; set; }

        /// <summary>
        /// View가 팝업인지 여부입니다.
        /// </summary>
        public bool IsPopup { get; set; }

        /// <summary>
        /// ViewModel DI에 사용된 고유 키 (싱글톤이 아닌 경우에만 사용)
        /// </summary>
        public string? UniqueKey { get; set; }
    }

    /// <summary>
    /// 주어진 타입 이름을 기반으로 View와 ViewModel을 생성하고 바인딩합니다.
    /// </summary>
    /// <param name="typeName">View의 전체 타입 이름</param>
    /// <param name="useSingletonView">ViewModel을 싱글턴으로 사용할지 여부</param>
    public static LoadedViewInfo LoadViewWithViewModel(string typeName, bool useSingletonView)
    {
        bool isPopup = typeName.EndsWith("_Popup", StringComparison.OrdinalIgnoreCase);
        string actualTypeName = isPopup ? typeName[..^6] : typeName;

        // 1. View 타입 얻기
        var viewType = Type.GetType(actualTypeName)
                      ?? AppDomain.CurrentDomain.GetAssemblies()
                          .SelectMany(a => a.GetTypes())
                          .FirstOrDefault(t => t.FullName == actualTypeName || t.Name == actualTypeName.Split('.').Last());

        // 2. View 인스턴스 생성
        UserControl viewInstance = viewType != null
            ? (Activator.CreateInstance(viewType) as UserControl ?? new PlaceholderView($"[No View Instance: {typeName}]"))
            : new PlaceholderView($"[No View Type: {typeName}]");

        // 3. ViewModel 타입 추론
        var viewModelTypeName = actualTypeName.Replace(".Pages.", ".ViewModels.") + "ViewModel";

        var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == viewModelTypeName || t.Name == viewModelTypeName.Split('.').Last());

        object? vmInstance = null;
        string? uniqueKey = null;

        if (viewModelType != null)
        {
            // 4. ViewModel Resolve
            if (!useSingletonView)
            {
                if (!ViewModelKeyCache.IndexMap.ContainsKey(typeName))
                    ViewModelKeyCache.IndexMap[typeName] = 0;

                int typeIndex = ViewModelKeyCache.IndexMap[typeName]++;
                uniqueKey = $"{typeName}_{typeIndex:D2}";
                vmInstance = VSContainer.Instance.Resolve(viewModelType, true, uniqueKey);
            }
            else
            {
                vmInstance = VSContainer.Instance.Resolve(viewModelType);
            }

            viewInstance.DataContext = vmInstance;
        }

        return new LoadedViewInfo
        {
            View = viewInstance,
            ViewModelType = viewModelType,
            IsPopup = isPopup,
            UniqueKey = uniqueKey
        };
    }
}
