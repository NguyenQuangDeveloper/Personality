using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AlarmConfig.Services.App_Setting
{
    public static class LoadConfigBase
    {
        public static T LoadConfig<T>(string fileName, string path = null) where T : new()
        {
            try
            {
                if (path == null)
                {
                    path = PathManager.Instance.PathConfig;
                }
                string filePath = System.IO.Path.Combine(path, fileName);
                if (File.Exists(filePath))
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        return JsonConvert.DeserializeObject<T>(file.ReadToEnd()) ?? new T();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return new T();
        }
    }

}
