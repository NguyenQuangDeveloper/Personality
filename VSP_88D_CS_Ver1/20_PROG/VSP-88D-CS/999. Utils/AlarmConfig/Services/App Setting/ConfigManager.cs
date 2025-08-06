using AlarmConfig.Models.AlarmSetup;
using AlarmConfig.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace AlarmConfig.Services.App_Setting
{
    public class ConfigManager
    {
        private static ConfigManager _instance = new ConfigManager();
        public static ConfigManager Instance => _instance;
        public bool IsLoaded = false;

        // Name file config json
        private const String CONFIG_ALARM_SETUP_NAME = "ConfigAlarmSetup.json";

        public Dictionary<SaveConfigObj, object> Params { get; set; } = new Dictionary<SaveConfigObj, object>();

        // Instance
        public AlarmSetting _alarmSetting;

        // Contructor
        private ConfigManager()
        {
        }
        private void Init()
        {
            Params[SaveConfigObj.SaveAlarmSetup] = new ParamItem<AlarmSetting>() { FileNameParam = CONFIG_ALARM_SETUP_NAME, Param = _alarmSetting };

        }
        public void StartUp()
        {
            Init();
            LoadConfig();
            LoadConfigAlarmSetting();
            IsLoaded = true;
        }

        public void LoadConfigAlarmSetting()
        {
            if(_alarmSetting == null || _alarmSetting.Alarms == null)
            {
                _alarmSetting = GetParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, true);
            }

            // Relace path.
            foreach (var item in _alarmSetting.Alarms)
            {
                string fileName = Path.GetFileName(item.Value.ImageMD.Path);
                string directory = Path.GetDirectoryName(item.Value.ImageMD.Path);

                if (string.IsNullOrEmpty(directory)) continue;

                string folderName = new DirectoryInfo(directory).Name;
                item.Value.ImageMD.Path = Path.Combine(Directory.GetCurrentDirectory(), PathManager.Instance.PathAlarmImageStore, fileName);
            }

            SaveParam<AlarmSetting>(SaveConfigObj.SaveAlarmSetup, _alarmSetting);
        }
        private void LoadConfig()
        {
            foreach (var param in Params.Values)
            {
                ((dynamic)param).Load();
            }
        }
        public T GetParam<T>(SaveConfigObj key) where T : class
        {
            if (Params.ContainsKey(key))
            {
                return ((dynamic)Params[key]).Param as T;
            }
            return null;
        }
        public T GetParam<T>(SaveConfigObj key, bool deepClone) where T : class
        {
            if (Params.ContainsKey(key))
            {
                try
                {
                    T param = ((dynamic)Params[key]).Param as T;

                    if (deepClone && param != null)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            Formatting = Formatting.Indented,
                            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true }
                        };

                        string json = JsonConvert.SerializeObject(param, settings);
                        Console.WriteLine("Serialized JSON: " + json);

                        return JsonConvert.DeserializeObject<T>(json, settings);
                    }

                    return param;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Serialization/Deserialization error: {ex.Message}");
                }
            }
            return null;
        }

        public bool SaveParam<T>(SaveConfigObj key, T param) where T : class
        {
            if (Params.ContainsKey(key))
            {
                try
                {
                    ((dynamic)Params[key]).Param = param;

                    ((dynamic)Params[key]).Save();

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"SaveParam error: {ex.Message}");
                }
            }
            return false;
        }
    }
    public class ParamItem<T> where T : new()
    {
        public string FileNameParam { get; set; }
        public T Param { get; set; }

        public void Save()
        {
            SaveConfigBase.SaveConfig(Param, FileNameParam);
        }
        public void Load()
        {
            Param = LoadConfigBase.LoadConfig<T>(FileNameParam);
        }
    }
}
