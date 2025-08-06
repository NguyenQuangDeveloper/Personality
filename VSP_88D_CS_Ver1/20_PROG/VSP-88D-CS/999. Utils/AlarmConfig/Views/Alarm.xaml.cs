using AlarmConfig.Services.App_Setting;
using AlarmConfig.Services.MultiLanguage;
using AlarmConfig.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using VSLibrary.Common.MVVM.Models;

namespace AlarmConfig.Views;

/// <summary>
/// Interaction logic for Alarm.xaml
/// </summary>
public partial class Alarm : UserControl
{
    public static readonly DependencyProperty AlarmListProperty =
        DependencyProperty.Register(nameof(AlarmList), typeof(ObservableCollection<ErrorItem>), typeof(Alarm),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAlarmListChanged));

    public static readonly DependencyProperty LanguageProperty =
        DependencyProperty.Register(
            nameof(Language),
            typeof(Languages),
            typeof(Alarm),
            new PropertyMetadata(Languages.English, OnLanguageChanged));

    private ObservableCollection<ErrorItem>? _alarmList;
    public ObservableCollection<ErrorItem> AlarmList
    {
        get => (ObservableCollection<ErrorItem>)GetValue(AlarmListProperty);
        set
        {
            SetValue(AlarmListProperty, value);
        }
    }

    public Languages Language
    {
        get => (Languages)GetValue(LanguageProperty);
        set
        {
            SetValue(LanguageProperty, value);
        }
    }

    private static void OnAlarmListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Alarm control)
            return;

        if (e.OldValue is ObservableCollection<ErrorItem> oldList)
            oldList.CollectionChanged -= control.AlarmList_CollectionChanged;

        if (e.NewValue is ObservableCollection<ErrorItem> newList)
        {
            newList.CollectionChanged += control.AlarmList_CollectionChanged;

            control.ViewModel.AlarmCodes = newList;
            control.ViewModel.AlarmCodeVM.LoadAlarmCodes(newList);
            control.ViewModel.AlarmCodeVM.LoadAlarmCodes();
        }
    }

    private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Alarm alarm && e.NewValue is Languages newLang)
        {
            Debug.WriteLine($"Language changed to: {newLang}");
            Languages = newLang;
            alarm.ViewModel.AlarmCodeVM.LoadAlarmCodes();

            LanguageViewModel.Instance.Refresh();
        }
    }

    private void AlarmList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Debug.WriteLine($"AlarmList Collection Changed: {e.Action}");

        ViewModel.AlarmCodes = AlarmList;
        ViewModel.AlarmCodeVM.LoadAlarmCodes(AlarmList);
    }


    public AlarmViewModel ViewModel { get; } = new AlarmViewModel();
    public static Languages Languages { get; set; }

    public Alarm()
    {
        InitializeComponent();

        this.Loaded += Alarm_Loaded;

        ConfigManager.Instance.StartUp();
    }

    private void Alarm_Loaded(object sender, RoutedEventArgs e)
    {
        if (AlarmList != null)
        {
            ViewModel.AlarmCodes = AlarmList;
            ViewModel.AlarmCodeVM.LoadAlarmCodes(AlarmList);
        }
    }
}
public enum Languages
{
    English,
    Korean,
    Chinese,
    Vietnamese
}



