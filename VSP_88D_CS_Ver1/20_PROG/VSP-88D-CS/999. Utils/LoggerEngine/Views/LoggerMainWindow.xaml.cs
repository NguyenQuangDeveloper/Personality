using LoggerEngine.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace LoggerEngine;

public partial class LoggerMainWindow : Window
{
    public LoggerViewModel ViewModel { get; }

    public LoggerMainWindow()
    {
        InitializeComponent();
        ViewModel = new LoggerViewModel();
        DataContext = ViewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}