using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.UIComponent.VSControls.ViewRegion;


/// <summary>
/// ViewModel 다중 인스턴스 생성을 방지하기 위한 고유 키 인덱스 캐시입니다.
/// - ViewModel 타입별로 인덱스를 관리하여 고유 Resolve 키를 생성할 때 사용됩니다.
/// - SingletonView가 아닌 경우에만 활용됩니다.
/// </summary>
public static class ViewModelKeyCache
{
    /// <summary>
    /// ViewModel 타입별 인덱스 맵입니다.
    /// key: ViewModel 타입 문자열 (예: "MyApp.ViewModels.MyPageViewModel")
    /// value: 현재까지 생성된 인덱스 (다음 Resolve에 사용됨)
    /// </summary>
    public static readonly Dictionary<string, int> IndexMap = new();
}