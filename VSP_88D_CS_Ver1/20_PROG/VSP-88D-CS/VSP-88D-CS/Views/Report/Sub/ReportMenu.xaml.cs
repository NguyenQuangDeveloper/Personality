using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VSP_88D_CS.Views.Report.Sub
{
    /// <summary>
    /// Interaction logic for ReportMenu.xaml
    /// </summary>
    public partial class ReportMenu : UserControl
    {
        public ReportMenu()
        {
            InitializeComponent();
        }
        public ICommand ReportMenuBtnCommand
        {
            get => (ICommand)GetValue(ReportMenuBtnCommandProperty);
            set => SetValue(ReportMenuBtnCommandProperty, value);

        }
        public static readonly DependencyProperty ReportMenuBtnCommandProperty = 
            DependencyProperty.Register(nameof(ReportMenuBtnCommand), typeof(ICommand),typeof(ReportMenu), new PropertyMetadata(null));
    }
}
