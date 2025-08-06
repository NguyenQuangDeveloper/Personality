using AlarmConfig.Models;
using AlarmConfig.Models.AlarmSetup;
using AlarmConfig.Models.Common;
using AlarmConfig.Services.App_Setting;
using AlarmConfig.Services.MultiLanguage;
using AlarmConfig.Views;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VSLibrary.Common.MVVM.Models;

namespace AlarmConfig.Services.Dialog;

/// <summar=>
/// Interaction logic for AlarmDialog.xaml
/// </summar=>
public partial class AlarmDialog : Window
{
    private Languages _alarmLanguage;
    private LanguageViewModel _languageVM => LanguageViewModel.Instance;

    private string _additionalStr = string.Empty;
    public string NameAlarm { get; set; }
    public string Cause { get; set; }
    public string Action { get; set; }
    public int AlarmCode { get; set; }
    public double DisplayWidth { get; set; }
    public double DisplayHeight { get; set; }
    public bool HideAlarm { get; set; } = true;

    public BitmapImage ImageAlarm { get; set; }
    public ObservableCollection<Shape> Shapes { get; set; } = new();

    public Dictionary<string, string> LanguageResources { get; set; }

    private double _heightDefault = 550;

    public AlarmDialog(ErrorItem errorItem, Languages alarmLanguage = Languages.English, string additionalStr = "")
    {
        InitializeComponent();
        this.DataContext = this;

        Alarm.Languages = alarmLanguage;
        _languageVM.Refresh();
        LanguageResources = _languageVM.LanguageResources;
        //LanguageViewModel.Instance.LanguageResourcesUpdated += Instance_LanguageResourcesUpdated;

        _additionalStr = additionalStr;
        GetAlarmInfo(errorItem);
        SetImageAlarm(errorItem.Code.ToString());

        GetSizeScreen();

        this.Loaded += AlarmDialog_Loaded;
        this.btnConfirm.Click += BtnConfirm_Click;
        this.btnHideAlarm.Click += BtnHideAlarm_Click;
    }

    private void Instance_LanguageResourcesUpdated()
    {
        LanguageResources = LanguageViewModel.Instance.LanguageResources;
    }

    private void AlarmDialog_Loaded(object sender, RoutedEventArgs e)
    {
        this.Focus();
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        HideAlarm = false;
        Close();
    }

    private void BtnHideAlarm_Click(object sender, RoutedEventArgs e)
    {
        HideAlarm = true;
        Close();
    }

    public bool ShowAlarm()
    {
        this.ShowDialog();

        return HideAlarm;
    }

    private void GetSizeScreen()
    {
        var titleWidth = MeasureTitleString().Width;

        if (titleWidth + 210 > DisplayWidth)
            this.Width = titleWidth + 230;
        else
            this.Width = DisplayWidth + 50;

        if (DisplayWidth <= 0)
            Width = 670;

        var heightTem = (SystemParameters.PrimaryScreenHeight - _heightDefault) + (DisplayHeight - 70);

        if (heightTem >= SystemParameters.PrimaryScreenHeight - 70)
            heightTem = SystemParameters.PrimaryScreenHeight - 70;
        this.Height = heightTem;
    }

    private void GetAlarmInfo(ErrorItem errorItem)
    {
        switch (Alarm.Languages)
        {
            case Languages.English:
                NameAlarm = errorItem.Name_E;
                Cause = errorItem.Cause_E;
                Action = errorItem.Action_E;
                AlarmCode = errorItem.Code;
                break;
            case Languages.Korean:
                NameAlarm = errorItem.Name;
                Cause = errorItem.Cause;
                Action = errorItem.Action;
                AlarmCode = errorItem.Code;
                break;
            case Languages.Chinese:
                NameAlarm = errorItem.Name_C;
                Cause = errorItem.Cause_C;
                Action = errorItem.Action_C;
                AlarmCode = errorItem.Code;
                break;
            case Languages.Vietnamese:
                NameAlarm = errorItem.Name_V;
                Cause = errorItem.Cause_V;
                Action = errorItem.Action_V;
                AlarmCode = errorItem.Code;
                break;
            default:
                break;
        }

        NameAlarm = $"[{_additionalStr}] {NameAlarm}";
        NameAlarm = NameAlarm.ToUpper();
    }

    private void SetImageAlarm(string code)
    {
        try
        {
            var alarmSetting = ConfigManager.Instance.GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup);

            alarmSetting.Alarms.TryGetValue(code, out var alarm);

            if (alarm == null)
            {
                SetHideImage(true);
                return;
            }

            if (string.IsNullOrEmpty(alarm.ImageMD.Path))
            {
                SetHideImage(true);
                return;
            }
            ImageAlarm = new BitmapImage(new Uri(alarm.ImageMD.Path, UriKind.RelativeOrAbsolute));

            DisplayWidth = alarm.ImageMD.WidthImage;
            DisplayHeight = alarm.ImageMD.HeightImage;

            SetHideImage(false);
            CreateShape(alarm);
        }
        catch (Exception ex)
        {
            SetHideImage(true);
        }
    }

    private void SetHideImage(bool status)
    {
        if (status)
        {
            canvas.Visibility = Visibility.Hidden;
            Height = _heightDefault;
        }
        else
        {
            canvas.Visibility = Visibility.Visible;
        }
    }

    private void CreateShape(AlarmControl alarmControl)
    {
        Shape shapeViewModel;
        for (int i = 0; i < alarmControl.ShapeMDs.Count; i++)
        {
            shapeViewModel = new Shape(alarmControl.ShapeMDs[i]);

            if (shapeViewModel.Animation)
                shapeViewModel.IsAnimating = true;

            Shapes.Add(shapeViewModel);
        }
    }
    protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }

    private Size MeasureTitleString()
    {
        var formattedText = new FormattedText(
            NameAlarm,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(txtAlarmName.FontFamily, txtAlarmName.FontStyle, txtAlarmName.FontWeight, txtAlarmName.FontStretch),
            txtAlarmName.FontSize,
            Brushes.Black,
            new NumberSubstitution(),
            VisualTreeHelper.GetDpi(txtAlarmName).PixelsPerDip);

        return new Size(formattedText.Width, formattedText.Height);
    }
}
