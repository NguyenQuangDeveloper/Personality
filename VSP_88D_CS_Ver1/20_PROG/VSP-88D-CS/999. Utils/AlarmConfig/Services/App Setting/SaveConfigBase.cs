using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AlarmConfig.Services.App_Setting
{
    public static class SaveConfigBase
    {
        public static bool SaveConfig<T>(T configObject, string configFileName, string path = null)
        {
            try
            {
                if (path == null)
                {
                    path = PathManager.Instance.PathConfig;
                }
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (configObject == null) return false;
                String filePath = Path.Combine(path, configFileName);
                if (string.IsNullOrEmpty(filePath)) return false;
                var js = JsonConvert.SerializeObject(configObject, Formatting.Indented);
                if (string.IsNullOrEmpty(js)) return false;
                File.WriteAllText(filePath, js);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
