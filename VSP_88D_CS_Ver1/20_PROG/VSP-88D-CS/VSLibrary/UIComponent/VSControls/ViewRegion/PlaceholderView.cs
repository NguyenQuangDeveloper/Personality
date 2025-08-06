using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VSLibrary.UIComponent.VSControls.ViewRegion;

/// <summary>
/// View 또는 ViewModel을 찾지 못했을 경우 대체로 표시되는 Fallback View입니다.
/// - 에러 메시지를 포함한 TextBlock을 중앙에 표시합니다.
/// - 디버깅 또는 UI 누락 여부 확인용으로 사용됩니다.
/// </summary>
public class PlaceholderView : UserControl
{
    /// <summary>
    /// 지정된 메시지를 포함한 대체 View를 생성합니다.
    /// </summary>
    /// <param name="message">사용자에게 표시할 오류 메시지입니다.</param>
    public PlaceholderView(string message)
    {
        this.Content = new TextBlock
        {
            Text = message,
            Foreground = Brushes.Red,
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(20)
        };
    }
}