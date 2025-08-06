using System.Resources;
using System.Windows;
using VSLibrary.Common.Log;

namespace VSLibrary.UIComponent
{
    public static class UIComponentManager
    {
        public static void Initialize()
        {
            RegisterResources();
        }

        private static void RegisterResources()
        {
            try
            {
                var app = Application.Current;

                // 디자인 타임에서는 Application.Current가 null일 수도 있음
                if (app == null) return;

                var isInDesignMode = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());

                // VsGridTemplates.xaml 등록
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/VSLibrary;component/UIComponent/VsGrids/VsGridTemplates.xaml", UriKind.Absolute)
                    });

                // KeyPadStyles.xaml 등록 추가
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/VSLibrary;component/UIComponent/Styles/KeyPadStyles.xaml", UriKind.Absolute)
                    });

                // VsButtonStyles.xaml 등록 추가
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/VSLibrary;component/UIComponent/Styles/VsButtonStyle.xaml", UriKind.Absolute)
                    });

                // VsButtonStyles.xaml 등록 추가
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/VSLibrary;component/UIComponent/Styles/VsCheckBoxStyle.xaml", UriKind.Absolute)
                    });
            }
            catch (Exception ex)
            {
                // 예외 발생 시 로그
                LogManager.Write($"[UIComponentManager] Resource 등록 실패: {ex.Message}",LogType.Error);
            }
        }
    }
}
