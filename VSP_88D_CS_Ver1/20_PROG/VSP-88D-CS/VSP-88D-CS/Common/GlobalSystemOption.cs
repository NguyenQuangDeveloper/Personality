using AlarmConfig.ViewModels;
using ConfigurationLib.Interfaces;
using ConfigurationLib.Services;
using ConfigurationLib.Shared;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Models;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.Models.Setting.SystemParameter;
using VSP_88D_CS.Sequence;
using VSP_88D_CS.VSP_COMMON;

namespace VSP_88D_CS.Common;

public interface IGlobalSystemOption
{
    #region PROPERTY
    public string RootPath { get; set; }
    public string DataPath { get; set; }
    public string LogPath { get; set; }
    public string ReportPath { get; set; }
    public string CimPath {  get; set; }

    //public ObservableCollection<ParameterItem> PlasmaItems { get; set; }
    public PlasmaOption PlasmaItems {  get; set; }  
    //public ObservableCollection<ParameterItem> SystemItems { get; set; }
    public SystemOption SystemItems { get; set; }

    // Maker system parameter
    public MfcParameter MfcParam { get; set; }
    public RfGenParameter RfGenParam { get; set; }
    public VacuumGaugeParameter VacGaugeParam { get; set; }
    public VacuumPumpParameter VacPumpParam { get; set; }
    public EtcParameter EtcParam { get; set; }
    public ObservableCollection<ErrorItem> Alarms { get; set; }

    //public SystemParamViewModel MakerParameters { get; set; }

    #endregion PROPERTY

    #region FUNCTION
    public void LoadPlasmaAndSystemParameter();
    #endregion FUNCTION
}

public class GlobalSystemOption : IGlobalSystemOption
{
    int cimType;
    const int NUMBER_OF_ALARMS = 135;
    readonly IConfigService _configService;

    #region PROPERTY
    // DEFAULT PATH
    enum VS_DIRs { DATA, LOG, REPORT, CIM, RES, ALARM }
    private string _rootPath;
    public string RootPath 
    { 
        get { return _rootPath; }
        set { _rootPath = value; }
    }

    private string _dataPath;
    public string DataPath
    {
        get { return _dataPath; }
        set { _dataPath = value; }
    }

    private string _logPath;
    public string LogPath
    {
        get { return _logPath; }
        set { _logPath = value; }
    }

    private string _reportPath;
    public string ReportPath
    {
        get { return _reportPath; }
        set { _reportPath = value; }
    }
    private string _cimPath;
    public string CimPath
    {
        get { return _cimPath; }
        set { _cimPath = value; }
    }
    private string _alarmPath;
    public string AlarmPath
    {
        get { return _alarmPath; }
        set { _alarmPath = value; }
    }

    public PlasmaOption _plasmaItems = new PlasmaOption();
    public PlasmaOption PlasmaItems
    {
        get => _plasmaItems;
        set => _plasmaItems = value;
    }

    public SystemOption _systemItems = new SystemOption();
    public SystemOption SystemItems
    {
        get => _systemItems;
        set => _systemItems = value;
    }
    public ObservableCollection<ErrorItem> Alarms { get; set; } = new();

    // Maker system parameter
    //private SystemParamViewModel _makerParameters;// = new SystemParamViewModel();
    private MfcParameter _mfcParam;
    public MfcParameter MfcParam
    {
        get => _mfcParam;
        set => _mfcParam = value;
    }
    private RfGenParameter _rfGenParam;
    public RfGenParameter RfGenParam
    {
        get => _rfGenParam;
        set => _rfGenParam = value;
    }
    private VacuumGaugeParameter _vacGaugeParam;
    public VacuumGaugeParameter VacGaugeParam
    {
        get => _vacGaugeParam;
        set => _vacGaugeParam = value;  
    }
    private VacuumPumpParameter _vacPumpParam;
    public VacuumPumpParameter VacPumpParam
    {
        get => _vacPumpParam;
        set => _vacPumpParam = value;
    }
    private EtcParameter _etcParam;
    public EtcParameter EtcParam
    {
        get => _etcParam;
        set => _etcParam = value;
    }

    //public SystemParamViewModel MakerParameters
    //{
    //    get => _makerParameters;
    //    set => _makerParameters = value;

    //}

    #endregion PROPERTY

    #region FUNCTION

    private VS_TOWER_OPTION TowerLamp = VS_TOWER_OPTION.Instance;
    public GlobalSystemOption()
    {
        _configService = VSContainer.Instance.Resolve<IConfigService>();

        SetPath();
        CreateStoreDirectories();
        LoadPlasmaAndSystemParameter();

        _mfcParam = new MfcParameter();
        VSContainer.Instance.RegisterInstance(_mfcParam);

        _rfGenParam = new RfGenParameter();
        VSContainer.Instance.RegisterInstance(_rfGenParam);

        _vacGaugeParam = new VacuumGaugeParameter();
        VSContainer.Instance.RegisterInstance(_vacGaugeParam);

        _vacPumpParam = new VacuumPumpParameter();
        VSContainer.Instance.RegisterInstance(_vacPumpParam);

        _etcParam = new EtcParameter();
        VSContainer.Instance.RegisterInstance(_etcParam);
        LoadMakerParam();
        LoadAlarms();
    }

    private async void LoadAlarms()
    {
        string alarmPath = Path.Combine(_alarmPath, "Alarms.json");
        var alarms = await _configService.LoadAsync<ObservableCollection<ErrorItem>>(ConfigStorageType.Json, alarmPath) ?? new();
        if (!alarms.Any() || alarms.Count != NUMBER_OF_ALARMS)
        {
            alarms = AlarmHelper.DefaultAlarms();
            await _configService.SaveAsync(alarms, ConfigStorageType.Json, alarmPath);
        }
        Alarms = alarms.DeepCloneObject();

        //AlarmViewModel alarmVM = (AlarmViewModel)VSContainer.Instance.MS_Services.GetServices<AlarmViewModel>().FirstOrDefault()!;
        //alarmVM.AlarmCodes = Alarms;

        //GenerateDefaultAlarmConfig();
    }


    private void CreateStoreDirectories()
    {
        try
        {
            if (!Directory.Exists(_rootPath))
            {
                Directory.CreateDirectory(_rootPath);
            }
            if (!Directory.Exists(_dataPath))
            {
                //Directory.CreateDirectory(_dataPath);
                string? sourcePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                sourcePath = Path.Combine(sourcePath, VS_DIRs.DATA.ToString());
                CopyFolder(sourcePath, _dataPath);
            }
            if (!Directory.Exists(_logPath))
            {
                Directory.CreateDirectory(_logPath);
            }
            if (!Directory.Exists(_reportPath))
            {
                Directory.CreateDirectory(_reportPath);
            }
            if (!Directory.Exists(_cimPath))
            {
                Directory.CreateDirectory(CimPath);
            }
            if (!Directory.Exists(_alarmPath))
            {
                Directory.CreateDirectory(_alarmPath);
            }

            //if (!Directory.Exists(GetResDir()))
            //{
            //    Directory.CreateDirectory(GetResDir());
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void SetPath()
    {
        //_rootPath = AppDomain.CurrentDomain.BaseDirectory;
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        bool bDExist = false;
        bool bDNotCD = false;

        foreach (DriveInfo drive in allDrives)
        {
            if (drive.Name.Equals("D:\\", StringComparison.OrdinalIgnoreCase))
            {
                bDExist = true;
                bDNotCD = drive.DriveType != DriveType.CDRom && drive.DriveType == DriveType.Fixed;
                break;
            }
        }

        if (bDExist && bDNotCD)
        {
            _rootPath = $"D:\\{GetProjectName()}\\";
        }
        else
        {
            _rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        }

        if(null != _rootPath)
        {
            _dataPath = Path.Combine(_rootPath, VS_DIRs.DATA.ToString());
            _logPath = Path.Combine(_rootPath, VS_DIRs.LOG.ToString());
            _reportPath = Path.Combine(_rootPath, VS_DIRs.REPORT.ToString());
            _cimPath = Path.Combine(_rootPath, VS_DIRs.CIM.ToString());
            _alarmPath = Path.Combine(_rootPath, VS_DIRs.ALARM.ToString());
        }    
    }

    public string GetProjectName()
    {
        string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string exeName = Path.GetFileName(exePath);
        string ext = Path.GetExtension(exePath);

        string projectName = exeName.Substring(0, exeName.Length - ext.Length);

        return projectName;
    }

    public static void CopyFolder(string sourcePath, string destinationPath, bool overwrite = true)
    {
        if (!Directory.Exists(sourcePath))
            throw new DirectoryNotFoundException($"Source folder not found: {sourcePath}");
        if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);

        foreach (string file in Directory.GetFiles(sourcePath))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(destinationPath, fileName);
            File.Copy(file, destFile, overwrite);
        }

        foreach (string directory in Directory.GetDirectories(sourcePath))
        {
            string dirName = Path.GetFileName(directory);
            string destSubDir = Path.Combine(destinationPath, dirName);
            CopyFolder(directory, destSubDir, overwrite);
        }
    }

    #region PARAMETER PLASMA AND SYSTEM
    public void LoadPlasmaAndSystemParameter()
    {
        //PlasmaItems.Clear();
        PlasmaItems.ParameterList.Clear();
        SystemItems.ParameterList.Clear();
        String FilePath = Path.Combine(DataPath, "OPTION.JSON");
        if (!File.Exists(FilePath))
        {
            //throw new FileNotFoundException("INI file does not exist.");
            LoadDefaultPlasmaAndSystemParameter();
            return;
        }

        //using (StreamReader reader = new StreamReader(FilePath))
        //{
        //    string? line;

        //    while ((line = reader.ReadLine()) != null)
        //    {
        //        line = line.Trim();
        //        if (string.IsNullOrWhiteSpace(line))
        //            continue;

        //        var parameterItem = JsonSerializer.Deserialize<ParameterItem>(line);
        //        if (parameterItem != null)
        //        {
        //            parameterItem.Value = parameterItem.Value;
        //            if ("PLASMA" == parameterItem.Section)
        //                PlasmaItems.ParameterList.Add((ParameterItem)parameterItem);
        //            else if("SYSTEM" == parameterItem.Section) 
        //                SystemItems.ParameterList.Add((ParameterItem)parameterItem);
        //        }
        //    }
        //    reader.Close();
        //}
        //if (0 == PlasmaItems.ParameterList.Count || 0 == SystemItems.ParameterList.Count)
        //    LoadDefaultPlasmaAndSystemParameter();
    }

    public void LoadDefaultPlasmaAndSystemParameter()
    {
        _plasmaItems.Default();
        _systemItems.Default();
        //ParameterItem parameterItem1 = new ParameterItem();
        //parameterItem1.Parameter = "RF REFLECT VALUE";
        //parameterItem1.Value = 100;
        //parameterItem1.Tooltip = "Input the RF REFLECT Value";
        //parameterItem1.Unit = "watt";
        //parameterItem1.IsVisible = true;
        //parameterItem1.Key = "RF_REFLECT_VALUE";
        //parameterItem1.Section = "PLASMA";
        //_plasmaItems.ParameterList.Add(parameterItem1);

        //ParameterItem parameterItem2 = new ParameterItem();
        //parameterItem2.Parameter = "RF REFLECT ERROR TIME (2~10)";
        //parameterItem2.Value = 5;
        //parameterItem2.Tooltip = "Input the RF REFLECT ERROR TIME (2~10) Value";
        //parameterItem2.Unit = "Sec.";
        //parameterItem2.IsVisible = true;
        //parameterItem2.Key = "RF_REFLECT_ERROR_TIME";
        //parameterItem2.Section = "PLASMA";
        //_plasmaItems.ParameterList.Add(parameterItem2);

        //ParameterItem parameterItem3 = new ParameterItem();
        //parameterItem3.Parameter = "MFC ERROR VALUE";
        //parameterItem3.Value = 10;
        //parameterItem3.Tooltip = "Input the MFC ERROR value.";
        //parameterItem3.Unit = "sccm";
        //parameterItem3.IsVisible = true;
        //parameterItem3.Section = "PLASMA";
        //parameterItem3.Key = "MFC_ERROR_VALUE";
        //_plasmaItems.ParameterList.Add(parameterItem3);

        //ParameterItem parameterItem4 = new ParameterItem();
        //parameterItem4.Parameter = "MFC ERROR TIME";
        //parameterItem4.Value = 15;
        //parameterItem4.Tooltip = "Input the MFC ERROR TIME value.";
        //parameterItem4.Unit = "sec.";
        //parameterItem4.IsVisible = true;
        //parameterItem4.Section = "PLASMA";
        //parameterItem4.Key = "MFC_ERROR_TIME";
        //_plasmaItems.ParameterList.Add(parameterItem4);

        //ParameterItem parameterItem5 = new ParameterItem();
        //parameterItem5.Parameter = "VENTILATION TIME (1000~30000)";
        //parameterItem5.Value = 1000;
        //parameterItem5.Tooltip = "Input the RF VENTILATION TIME value.";
        //parameterItem5.Unit = "sec.";
        //parameterItem5.IsVisible = true;
        //parameterItem5.Section = "PLASMA";
        //parameterItem5.Key = "VENTILATION_TIME";
        //_plasmaItems.ParameterList.Add(parameterItem5);

        //ParameterItem parameterItem6 = new ParameterItem();
        //parameterItem6.Parameter = "VACUUM ERROR TIME";
        //parameterItem6.Value = 1000;
        //parameterItem6.Tooltip = "Input the VACUUM ERROR TIME value.";
        //parameterItem6.Unit = "sec.";
        //parameterItem6.IsVisible = true;
        //parameterItem6.Section = "PLASMA";
        //parameterItem6.Key = "VACUUM_ERROR_TIME";
        //_plasmaItems.ParameterList.Add(parameterItem6);

        //ParameterItem parameterItem7 = new ParameterItem();
        //parameterItem7.Parameter = "MANUAL RF ON";
        //parameterItem7.Value = true;
        //parameterItem7.Type = ParameterType.CheckBox;
        //parameterItem7.CheckedText = "Yes";
        //parameterItem7.UncheckedText = "No";
        //parameterItem7.Tooltip = "Select how to manual RF on.";
        //parameterItem7.IsVisible = true;
        //parameterItem7.IsEditable = true;
        //parameterItem7.Section = "PLASMA";
        //parameterItem7.Key = "MANUAL_RF_ON";
        //_plasmaItems.ParameterList.Add(parameterItem7);

        /////////////////////////////////////////////////////////////
        //ParameterItem sysItem1 = new ParameterItem();
        //sysItem1.Parameter = "BUZZER OFF TIME (3~60)";
        //sysItem1.Value = 3;
        //sysItem1.Unit = "sec.";
        //sysItem1.Tooltip = "Input the BUZZER OFF TIME (3~60) value.";
        //sysItem1.IsVisible = true;
        //sysItem1.Section = "SYSTEM";
        //sysItem1.Key = "BUZZER_OFF_TIME";
        //_systemItems.Add(sysItem1);

        //ParameterItem sysItem2 = new ParameterItem();
        //sysItem2.Parameter = "OUT STRIP EMPTY CHECK TIME";
        //sysItem2.Value = 500;
        //sysItem2.Unit = "msec.";
        //sysItem2.Tooltip = "Input the OUT STRIP EMPTY CHECK TIME value.";
        //sysItem2.IsVisible = true;
        //sysItem2.Section = "SYSTEM";
        //sysItem2.Key = "OUT_STRIP_EMPTY_CHECK_TIME";
        //_systemItems.Add(sysItem2);

        //ParameterItem sysItem3 = new ParameterItem();
        //sysItem3.Parameter = "SERVO POSITION CHANGE ENABLE";
        //sysItem3.Value = false;
        //sysItem3.Tooltip = "Input the SERVO POSITION CHANGE ENABLE value.";
        //sysItem3.Type = ParameterType.CheckBox;
        //sysItem3.IsVisible = true;
        //sysItem3.IsEditable = true;
        //sysItem3.Section = "SYSTEM";
        //sysItem3.Key = "SERVO_POSITION_CHANGE_ENABLE";
        //_systemItems.Add(sysItem3);

        //ParameterItem sysItem4 = new ParameterItem();
        //sysItem4.Parameter = "INDEX PUSHER UP AFTER LOADING";
        //sysItem4.Value = false;
        //sysItem4.Tooltip = "Input the INDEX PUSHER UP AFTER LOADING value.";
        //sysItem4.Type = ParameterType.CheckBox;
        //sysItem4.IsVisible = true;
        //sysItem4.IsEditable = true;
        //sysItem4.Section = "SYSTEM";
        //sysItem4.Key = "INDEX_PUSHER_UP_AFTER_LOADING";
        //_systemItems.Add(sysItem4);

        //ParameterItem sysItem5 = new ParameterItem();
        //sysItem5.Parameter = "LANGUAGE";
        //sysItem5.Value = 1;
        //sysItem5.Tooltip = "Select the LANGUAGE value.";
        //sysItem5.Type = ParameterType.ComboBox;
        //sysItem5.ComboBoxItems.Clear();
        //sysItem5.ComboBoxItems.Add("KOREAN");
        //sysItem5.ComboBoxItems.Add("ENGLISH");
        //sysItem5.ComboBoxItems.Add("CHINESE");
        //sysItem5.IsVisible = true;
        //sysItem5.IsEditable = true;
        //sysItem5.Section = "SYSTEM";
        //sysItem5.Key = "LANGUAGE";
        //_systemItems.Add(sysItem5);

        //ParameterItem sysItem6 = new ParameterItem();
        //sysItem6.Parameter = "DOOR SENSOR SKIP DURING MANUAL";
        //sysItem6.Value = false;
        //sysItem6.Tooltip = "Select the DOOR SENSOR SKIP DURING MANUAL value.";
        //sysItem6.Type = ParameterType.CheckBox;
        //sysItem6.IsVisible = true;
        //sysItem6.IsEditable = true;
        //sysItem6.Section = "SYSTEM";
        //sysItem6.Key = "DOOR_SENSOR_SKIP_DURING_MANUAL";
        //_systemItems.Add(sysItem6);

        //ParameterItem sysItem7 = new ParameterItem();
        //sysItem7.Parameter = "UNLOADING BUFFER KEEP RIGHT";
        //sysItem7.Value = false;
        //sysItem7.Tooltip = "Select the UNLOADING BUFFER KEEP RIGHT value.";
        //sysItem7.Type = ParameterType.CheckBox;
        //sysItem7.IsVisible = true;
        //sysItem7.IsEditable = true;
        //sysItem7.Section = "SYSTEM";
        //sysItem7.Key = "UNLOADING_BUFFER_KEEP_RIGHT";
        //_systemItems.Add(sysItem7);

        //ParameterItem sysItem8 = new ParameterItem();
        //sysItem8.Parameter = "CHAMBER CLOSE AFTER LOT END";
        //sysItem8.Value = false;
        //sysItem8.Tooltip = "Select the CHAMBER CLOSE AFTER LOT END value.";
        //sysItem8.Type = ParameterType.CheckBox;
        //sysItem8.IsVisible = true;
        //sysItem8.IsEditable = true;
        //sysItem8.Section = "SYSTEM";
        //sysItem8.Key = "CHAMBER_CLOSE_AFTER_LOT_END";
        //_systemItems.Add(sysItem8);

        //ParameterItem sysItem9 = new ParameterItem();
        //sysItem9.Parameter = "UNLOAD SENSOR TYPE NORMAL CLOSE";
        //sysItem9.Value = false;
        //sysItem9.Tooltip = "Select the UNLOAD SENSOR TYPE NORMAL CLOSE value.";
        //sysItem9.Type = ParameterType.CheckBox;
        //sysItem9.IsVisible = true;
        //sysItem9.IsEditable = true;
        //sysItem9.Section = "SYSTEM";
        //sysItem9.Key = "UNLOAD_SENSOR_TYPE_NORMAL_CLOSE";
        //_systemItems.Add(sysItem9);

        //ParameterItem sysItem10 = new ParameterItem();
        //sysItem10.Parameter = "AUTOMATION";
        //sysItem10.Value = 0;
        //sysItem10.Tooltip = "Select the AUTOMATION value.";
        //sysItem10.Type = ParameterType.ComboBox;
        //sysItem10.ComboBoxItems.Clear();
        //sysItem10.ComboBoxItems.Add("NONE");
        //sysItem10.ComboBoxItems.Add("ATK PLS");
        //sysItem10.ComboBoxItems.Add("SIGNETICS");
        //sysItem10.ComboBoxItems.Add("GEM");
        //sysItem10.IsVisible = true;
        //sysItem10.IsEditable = true;
        //sysItem10.Section = "SYSTEM";
        //sysItem10.Key = "AUTOMATION";
        //_systemItems.Add(sysItem10);

        //ParameterItem sysItem11 = new ParameterItem();
        //sysItem11.Parameter = "GAS INJECTION WHEN VACUUM START";
        //sysItem11.Value = false;
        //sysItem11.Tooltip = "Select the GAS INJECTION WHEN VACUUM START value.";
        //sysItem11.Type = ParameterType.CheckBox;
        //sysItem11.IsVisible = true;
        //sysItem11.IsEditable = true;
        //sysItem11.Section = "SYSTEM";
        //sysItem11.Key = "GAS_INJECTION_WHEN_VACUUM_START";
        //_systemItems.Add(sysItem11);

        //ParameterItem sysItem12 = new ParameterItem();
        //sysItem12.Parameter = "ID READING DELAY";
        //sysItem12.Value = false;
        //sysItem12.Tooltip = "Select the ID READING DELAY.";
        //sysItem12.Type = ParameterType.CheckBox;
        //sysItem12.IsVisible = true;
        //sysItem12.IsEditable = true;
        //sysItem12.Section = "SYSTEM";
        //sysItem12.Key = "ID_READING_DELAY";
        //_systemItems.Add(sysItem12);

        //ParameterItem sysItem13 = new ParameterItem();
        //sysItem13.Parameter = "ID READING DELAY VALUE";
        //sysItem13.Value = 1000;
        //sysItem13.Tooltip = "Select the ID READING DELAY value.";
        //sysItem13.Unit = "msec";
        //sysItem13.IsVisible = true;
        //sysItem13.Section = "SYSTEM";
        //sysItem13.Key = "ID_READING_DELAY_VALUE";
        //_systemItems.Add(sysItem13);
    }
    #endregion PARAMETER PLASMA AND SYSTEM

    public void LoadMakerParam()
    {
        string FilePath = Path.Combine(_dataPath, "SYSTEM.JSON");
        if (!File.Exists(FilePath))
        {
            return;
        }
        using (StreamReader reader = new StreamReader(FilePath))
        {
            int lineIndex = 0;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var type = (eSystemOptType)lineIndex;
                switch (type)
                {
                    case eSystemOptType.MFC:
                        var paramMfc = JsonSerializer.Deserialize<MfcParameter>(line);
                        if (null != paramMfc)
                            _mfcParam = paramMfc;
                        break;
                    case eSystemOptType.RF_GEN:
                        var paramRf = JsonSerializer.Deserialize<RfGenParameter>(line);
                        if (null != paramRf)
                            _rfGenParam = paramRf;
                        break;
                        break;
                    case eSystemOptType.VAC_GAUGE:
                        var paramGau = JsonSerializer.Deserialize<VacuumGaugeParameter>(line);
                        if (null != paramGau)
                            _vacGaugeParam = paramGau;
                        break;
                    case eSystemOptType.VAC_PUMP:
                        var paramPump = JsonSerializer.Deserialize<VacuumPumpParameter>(line);
                        if (null != paramPump)
                            _vacPumpParam = paramPump;
                        break;
                    case eSystemOptType.ETC:
                        var paramEtc = JsonSerializer.Deserialize<EtcParameter>(line);
                        if (null != paramEtc)
                            _etcParam = paramEtc;
                        break;
                    default:
                        break;
                }
                lineIndex++;
            }
        }
    }
    public int GetCimType() 
    { 
        return cimType = 1; 
    }
    void SetCimType(int Val)		
    {
        cimType = Val; 
    }
    #endregion FUNCTION
}
