using LoggerEngine.Models;
using LoggerEngine.ViewModels;
using System.Windows;

namespace LoggerEngine.Views;

/// <summary>
/// Interaction logic for LoggerOptionWindow.xaml
/// </summary>
public partial class LoggerOptionsWindow : Window
{
    public LoggerOptionsWindow( VSLoggerSettingModel vsLoggerSetting)
    {
        InitializeComponent();
        DataContext = new LoggerOptionsViewModel(vsLoggerSetting);
    }
}
