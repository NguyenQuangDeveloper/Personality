using AlarmConfig.Models;
using AlarmConfig.Models.AlarmSetup;
using AlarmConfig.Models.Common;
using AlarmConfig.Services.App_Setting;
using AlarmConfig.Services.Dialog;
using AlarmConfig.ViewModels.Common;
using AlarmConfig.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VSLibrary.Common.MVVM.Models;
using Shape = AlarmConfig.Models.Shape;

namespace AlarmConfig.ViewModels.Lefts;

public class AlarmCodeViewModel : BaseViewModel
{
    public Button SelectedAlarm { get; set; }

    private ObservableCollection<Button> _alarmCodes = new();
    public ObservableCollection<Button> AlarmCodes
    {
        get => _alarmCodes;
        set => SetProperty(ref _alarmCodes, value);
    }

    public ICommand AlarmCommand { get; set; }
    public ICommand DisplayAlarmCommand { get; set; }

    private AlarmViewModel _alarmViewModel;

    public AlarmCodeViewModel(AlarmViewModel alarmViewModel)
    {
        _alarmViewModel = alarmViewModel;
        AlarmCommand = new RelayCommand<Button>(AlarmExecute);
        DisplayAlarmCommand = new RelayCommand(DisplayAlarm);
    }

    private void DisplayAlarm()
    {
        if (SelectedAlarm == null) 
            return;

        var getAlarmItem = _alarmViewModel.AlarmCodes.FirstOrDefault(x => x.Code == Convert.ToInt32(SelectedAlarm.Content));

        AlarmDialog alarmDialog = new(getAlarmItem!);

        var result = alarmDialog.ShowAlarm();

        if(result)
        {
            // Hide Alarm
        }    
        else
        {
            // Comfirm alarm
        }    
    }

    public void LoadAlarmCodes(ObservableCollection<ErrorItem> AlarmList)
    {
        if (AlarmCodes != null && AlarmCodes.Count > 0) return;

        AlarmCodes = new();

        if (AlarmList == null)
        {
            return;
        }

        foreach (var alarmCode in AlarmList)
        {
            Button button = new Button();

            button.Content = alarmCode.Code.ToString();
            button.Message = alarmCode.Cause;
            button.Solution = alarmCode.Action;
            button.Name = alarmCode.Name;

            AlarmCodes.Add(button);
        }
    }

    public void LoadAlarmCodes()
    {
        AlarmCodes = new();

        if (_alarmViewModel.AlarmCodes == null)
        {
            return;
        }

        foreach (var alarmCode in _alarmViewModel.AlarmCodes)
        {
            Button button = new Button();

            if (Alarm.Languages == Languages.Korean)
            {
                button.Content = alarmCode.Code.ToString();
                button.Message = alarmCode.Cause;
                button.Solution = alarmCode.Action;
                button.Name = alarmCode.Name;
            }
            else if (Alarm.Languages == Languages.English)
            {
                button.Content = alarmCode.Code.ToString();
                button.Message = alarmCode.Cause_E;
                button.Solution = alarmCode.Action_E;
                button.Name = alarmCode.Name_E;
            }
            else if (Alarm.Languages == Languages.Chinese)
            {
                button.Content = alarmCode.Code.ToString();
                button.Message = alarmCode.Cause_C;
                button.Solution = alarmCode.Action_C;
                button.Name = alarmCode.Name_C;
            }
            else if (Alarm.Languages == Languages.Vietnamese)
            {
                button.Content = alarmCode.Code.ToString();
                button.Message = alarmCode.Cause_V;
                button.Solution = alarmCode.Action_V;
                button.Name = alarmCode.Name_V;
            }

            AlarmCodes.Add(button);
        }

        _alarmViewModel.AlarmInfoVM.GetAlarmNames();
    }

    public void AlarmExecute(Button button)
    {
        _alarmViewModel.AlarmInfoVM.SetAlarmMessage(button,true);

        ClearSelectionAlarm();
        button.IsSelected = true;

        _alarmViewModel.FileToolVM.alarmSetting = ConfigManager.Instance.GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, true);

        SelectedAlarm = button;

        var alarm = GetOrCreateAlarm(button.Content);

        ResetDrawingArea();
        LoadImageForAlarm(alarm);
        LoadShapesForAlarm(alarm);
    }

    public void AlarmExecute(Button button, bool isSetAlarmInfo)
    {
        ClearSelectionAlarm();
        button.IsSelected = true;

        _alarmViewModel.FileToolVM.alarmSetting = ConfigManager.Instance.GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, true);

        SelectedAlarm = button;

        var alarm = GetOrCreateAlarm(button.Content);

        ResetDrawingArea();
        LoadImageForAlarm(alarm);
        LoadShapesForAlarm(alarm);
    }

    private void ClearSelectionAlarm()
    {
        foreach (var alarmCode in AlarmCodes)
        {
            alarmCode.IsSelected = false;
        }
    }

    private AlarmControl GetOrCreateAlarm(string alarmKey)
    {
        var alarmSetting = ConfigManager.Instance.GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, true);

        if (!alarmSetting.Alarms.TryGetValue(alarmKey, out var alarm))
        {
            alarm = CreateDefaultAlarm();
            alarmSetting.Alarms[alarmKey] = alarm;
        }
        return alarm;
    }

    private AlarmControl CreateDefaultAlarm()
    {
        var alarmControl = new AlarmControl();
        alarmControl.ImageMD.Path = string.Empty;
        alarmControl.ShapeMDs = new List<Shape>();
        return alarmControl;
    }

    private void ResetDrawingArea()
    {
        _alarmViewModel.DrawingAreaVM.Shapes.Clear();
        _alarmViewModel.DrawingAreaVM.ImageAlarm = null;
    }

    private void LoadImageForAlarm(AlarmControl alarm)
    {
        try
        {
            _alarmViewModel.DrawingAreaVM.DisplayHeight = alarm.ImageMD.HeightImage;
            _alarmViewModel.DrawingAreaVM.DisplayWidth = alarm.ImageMD.WidthImage;

            _alarmViewModel.ImageToolVM.Height = alarm.ImageMD.HeightImage;
            _alarmViewModel.ImageToolVM.Width = alarm.ImageMD.WidthImage;

            if (!string.IsNullOrEmpty(alarm.ImageMD.Path))
            {
                _alarmViewModel.DrawingAreaVM.ImageAlarm = new BitmapImage(new Uri(alarm.ImageMD.Path, UriKind.RelativeOrAbsolute));
            }
        }
        catch(Exception ex)
        {
            _alarmViewModel.FileToolVM.RemoveImage(alarm);
        }
    }

    private void LoadShapesForAlarm(AlarmControl alarm)
    {
        foreach (var shapeMD in alarm.ShapeMDs)
        {
            _alarmViewModel.DrawingAreaVM.Shapes.Add(new Shape(shapeMD));
        }
    }

}
