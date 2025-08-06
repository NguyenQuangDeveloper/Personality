using AlarmConfig.Models;
using AlarmConfig.Models.AlarmSetup;
using AlarmConfig.Models.Common;
using AlarmConfig.Services.App_Setting;
using AlarmConfig.ViewModels.Common;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AlarmConfig.ViewModels.Tops;

public class FileToolViewModel : BaseViewModel
{
    private SelectCreateImage _selectCreateImage;
    public SelectCreateImage SelectCreateImage
    {
        get => _selectCreateImage;
        set => SetProperty(ref _selectCreateImage, value);
    }

    private bool _isAnimating;
    public bool IsAnimating
    {
        get => _isAnimating;
        set
        {
            SetProperty(ref _isAnimating, value);
        }
    }

    public string ImageFolder => PathManager.Instance.PathAlarmImageStore;

    public ICommand SaveCommand { get; set; }
    public ICommand CreateImageCommand { get; set; }
    public ICommand DeleteImageCommand { get; set; }
    public ICommand SelectImageFileCommand { get; set; }
    public ICommand ToggleAnimationCommand { get; set; }

    private AlarmViewModel _alarmViewModel;
    public AlarmSetting alarmSetting;

    public FileToolViewModel(AlarmViewModel alarmViewModel)
    {
        _alarmViewModel = alarmViewModel;

        SaveCommand = new RelayCommand(SaveExecute);
        CreateImageCommand = new RelayCommand(CreateImageExecute);
        DeleteImageCommand = new RelayCommand(DeleteImageExecute);
        SelectImageFileCommand = new RelayCommand(SelectImageFileExecute);
        ToggleAnimationCommand = new RelayCommand(ToggleAnimationExecute);
    }

    private void SaveExecute()
    {
        var dialogReult = MessageBox.Show(LanguageResources["msg_DoYouWantToSave"],
            LanguageResources["Question"], MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (dialogReult == MessageBoxResult.No) return;

        if (_alarmViewModel.AlarmCodeVM.SelectedAlarm == null) return;

        alarmSetting.Alarms.TryGetValue(_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content, out AlarmControl alarmControl);

        if (alarmControl == null) alarmControl = new AlarmControl();
        else alarmControl.ShapeMDs = new List<Shape>();

        for (int i = 0; i < _alarmViewModel.DrawingAreaVM.Shapes.Count; i++)
        {
            if (alarmControl == null)
            {
                alarmControl!.ImageMD.Path = "";
                alarmSetting.Alarms.Add(_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content, alarmControl);
                continue;
            }
            alarmControl.ShapeMDs.Add(_alarmViewModel.DrawingAreaVM.Shapes[i]);
            alarmSetting.Alarms[_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content] = alarmControl;
        }

        alarmControl.ImageMD.HeightImage = _alarmViewModel.DrawingAreaVM.DisplayHeight;
        alarmControl.ImageMD.WidthImage = _alarmViewModel.DrawingAreaVM.DisplayWidth;

        ConfigManager.Instance.SaveParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, alarmSetting);
    }

    private void CreateImageExecute()
    {
        if (!ConfirmCreate()) return;

        var imageDirectory = PathManager.Instance.PathAlarmImageStore;
        var fileNames = GetImageFileNames(imageDirectory);
        if (fileNames.Count == 0)
        {
            MessageBox.Show(LanguageResources["msg_NoAvailableImage"], LanguageResources["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (alarmSetting == null)
            alarmSetting = ConfigManager.Instance.GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, true);

        UpdateAlarmsWithImages(alarmSetting, fileNames, imageDirectory, out int matchItems);

        if (matchItems == 0)
        {
            if (SelectCreateImage == SelectCreateImage.WithAlarmCode)
                MessageBox.Show(LanguageResources["msg_NoValidCodeMappedImage"], LanguageResources["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                MessageBox.Show(LanguageResources["msg_NoValidNameMappedImage"], LanguageResources["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ConfigManager.Instance.SaveParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, alarmSetting);
        ShowSuccessMessage();
    }

    private void DeleteImageExecute()
    {
        if (_alarmViewModel.AlarmCodeVM.SelectedAlarm == null) 
            return;

        var dialogReult = MessageBox.Show(LanguageResources["msg_DoYouWantToDeleteImage"],
        LanguageResources["Question"], MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (dialogReult == MessageBoxResult.No) 
            return;

        alarmSetting.Alarms.TryGetValue(_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content, out AlarmControl alarmControl);

        RemoveImage(alarmControl!);
    }

    public void RemoveImage(AlarmControl alarmControl)
    {
        if (alarmControl == null) return;

        alarmSetting.Alarms.Remove(_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content);

        ConfigManager.Instance.SaveParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, alarmSetting);
        _alarmViewModel.AlarmCodeVM.AlarmExecute(_alarmViewModel.AlarmCodeVM.SelectedAlarm);
    }

    private void SelectImageFileExecute()
    {
        if (_alarmViewModel.AlarmCodeVM.SelectedAlarm == null)
        {
            MessageBox.Show(LanguageResources["msg_PleaseSelectAnAlarmCode"] + "!!!", LanguageResources["Warning"], MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedFilePath = OpenImageFileDialog();
        if (string.IsNullOrEmpty(selectedFilePath)) return;

        if (!IsFileExistInFolder(PathManager.Instance.PathAlarmImageStore, Path.GetFileName(selectedFilePath)))
        {
            selectedFilePath = CopyImageWithAutoRename(selectedFilePath, PathManager.Instance.PathAlarmImageStore);
        }

        var fileSavePath = GetSavedImagePath(selectedFilePath);

        UpdateImageAlarm(fileSavePath);
        UpdateAlarmSetting(alarmSetting, fileSavePath);
    }

    public bool IsFileExistInFolder(string folderPath, string fileName)
    {
        if (!Directory.Exists(folderPath))
            return false;

        string fullPath = Path.Combine(folderPath, fileName);
        return File.Exists(fullPath);
    }

    public string CopyImageWithAutoRename(string sourcePath, string destinationFolder)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source image not found.", sourcePath);

        if (!Directory.Exists(destinationFolder))
            Directory.CreateDirectory(destinationFolder);

        string fileName = Path.GetFileName(sourcePath);
        string destPath = Path.Combine(destinationFolder, fileName);

        int count = 1;
        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);

        while (File.Exists(destPath))
        {
            string newFileName = $"{nameWithoutExt}_{count}{extension}";
            destPath = Path.Combine(destinationFolder, newFileName);
            count++;
        }

        File.Copy(sourcePath, destPath);

        return destPath;
    }

    private void ToggleAnimationExecute()
    {
        IsAnimating = !IsAnimating;

        _alarmViewModel.DrawingAreaVM.ExecuteAnimation(IsAnimating);
    }

    // Process
    private bool ConfirmCreate()
    {
        return MessageBox.Show(LanguageResources["msg_DoYouWantToCreate"], LanguageResources["Question"], MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    private List<string> GetImageFileNames(string directory)
    {
        if (!Directory.Exists(directory)) return new List<string>();

        return Directory.GetFiles(directory).Select(Path.GetFileName).ToList()!;
    }

    private void UpdateAlarmsWithImages(AlarmSetting alarmSetting, List<string> fileNames, string imageDirectory, out int matchItems)
    {
        matchItems = 0;
        foreach (var item in _alarmViewModel.AlarmCodeVM.AlarmCodes)
        {
            string matchKey = GetMatchKey(item);
            string dictKey = item.Content;

            var matchedFile = fileNames.FirstOrDefault(file => (file.Split('.'))[0] == matchKey);
            if (matchedFile == null) 
                continue;

            matchItems++;
            if (!alarmSetting.Alarms.TryGetValue(dictKey, out var alarmControl))
            {
                alarmControl = CreateNewAlarm(imageDirectory, matchedFile);
                alarmSetting.Alarms[dictKey] = alarmControl;
            }
            else
            {
                alarmControl.ImageMD.Path = Path.Combine(imageDirectory, matchedFile);
            }

            UpdateImageAlarm(alarmControl.ImageMD.Path);
            UpdateAlarmSetting(alarmSetting!, alarmControl.ImageMD.Path);
        }
    }

    private string GetMatchKey(Button item)
    {
        return SelectCreateImage switch
        {
            SelectCreateImage.WithAlarmName => item.Name,
            SelectCreateImage.WithAlarmCode => item.Content,
            _ => item.Content
        };
    }

    private AlarmControl CreateNewAlarm(string directory, string fileName)
    {
        return new AlarmControl
        {
            ImageMD = new ImageMD { Path = Path.Combine(directory, fileName) },
            ShapeMDs = new List<Shape>()
        };
    }

    private void ShowSuccessMessage()
    {
        MessageBox.Show(LanguageResources["msg_CreateAlarmFileSuccess"] + "!!!", LanguageResources["Info"], MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private string OpenImageFileDialog()
    {
        string imageAlarmDirectory = PathManager.Instance.PathAlarmImageStore;

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            InitialDirectory = imageAlarmDirectory,
            Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
            RestoreDirectory = true
        };

        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }

    private string GetSavedImagePath(string selectedFilePath)
    {
        string selectedFileName = Path.GetFileName(selectedFilePath);
        return Path.Combine(PathManager.Instance.PathAlarmImageStore, selectedFileName);
    }

    private void UpdateImageAlarm(string fileSavePath)
    {
        _alarmViewModel.DrawingAreaVM.ImageAlarm = new BitmapImage(new Uri(fileSavePath, UriKind.RelativeOrAbsolute));
    }

    private void UpdateAlarmSetting(AlarmSetting alarmSetting, string fileSavePath)
    {
        if (!alarmSetting.Alarms.TryGetValue(_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content, out AlarmControl alarmControl))
        {
            alarmControl = CreateNewAlarm(fileSavePath);
            alarmSetting.Alarms[_alarmViewModel.AlarmCodeVM.SelectedAlarm.Content] = alarmControl;
        }
        else
        {
            alarmControl.ImageMD.Path = fileSavePath;
            alarmControl.ShapeMDs.Clear();
        }

        _alarmViewModel.DrawingAreaVM.Shapes = new ObservableCollection<Shape>();
    }

    private AlarmControl CreateNewAlarm(string fileSavePath)
    {
        return new AlarmControl
        {
            ImageMD = new ImageMD { Path = fileSavePath },
            ShapeMDs = new List<Shape>()
        };
    }
}
