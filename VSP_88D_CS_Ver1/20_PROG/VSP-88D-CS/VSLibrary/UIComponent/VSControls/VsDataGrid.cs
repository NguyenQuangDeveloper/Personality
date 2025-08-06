using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.VSControls;

/// <summary>
/// \class VsDataGrid
/// \brief VSLibrary용 커스텀 DataGrid 컨트롤의 기본 골격입니다.
/// 
/// 스타일 자동 병합 및 추후 MVVM 확장 (컬럼 바인딩, RowCommand 등)을 위한 구조체입니다.
/// </summary>
public class VsDataGrid : DataGrid
{
    static VsDataGrid()
    {
        // 기본 스타일 키 오버라이드
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsDataGrid),
            new FrameworkPropertyMetadata(typeof(VsDataGrid)));

        // 리소스 중복 등록 방지
        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsDataGridStyle.xaml", UriKind.RelativeOrAbsolute);

        if (Application.Current != null)
        {
            bool alreadyAdded = Application.Current.Resources.MergedDictionaries
                .OfType<ResourceDictionary>()
                .Any(x => x.Source != null && x.Source.Equals(uri));

            if (!alreadyAdded)
            {
                var dict = new ResourceDictionary { Source = uri };
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
        }
    }

    // TODO: SelectionChangedCommand, ColumnTemplates 등 추후 확장 예정
}