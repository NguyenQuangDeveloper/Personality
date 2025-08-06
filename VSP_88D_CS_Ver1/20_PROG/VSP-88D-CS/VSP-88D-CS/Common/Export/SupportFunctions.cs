using System.IO;
using System.Windows;
using Newtonsoft.Json;
using VSLibrary.UIComponent.MessageBox;

namespace VSP_88D_CS.Common.Export
{
    public static class SupportFunctions
    {
        private static readonly object _lock = new object();
        private static readonly string _defaultFolder = AppDomain.CurrentDomain.BaseDirectory + "DATA";
        public static T LoadJsonFile<T>(string fileName)
        {
            string defaultFile = fileName;
            if (File.Exists(defaultFile))
            {
                using (FileStream fileStream = new FileStream(defaultFile, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string rawString = reader.ReadToEnd();
                        //   var tempData = JsonConvert.DeserializeObject<List<ViewStepModel>>(rawString);
                        try
                        {
                            var tempDataLoad = JsonConvert.DeserializeObject<T>(rawString);
                            return tempDataLoad;
                        }
                        catch (Exception)
                        {


                        }

                    }
                }
            }
            return default;

        }
        public static T LoadJsonFile<T>(string fileName, string path)
        {
            string defaultFolder = path;
            if (string.IsNullOrEmpty(defaultFolder))
            {
                defaultFolder = _defaultFolder;
            }

            string defaultFile = $"{defaultFolder}\\{fileName}";
            if (File.Exists(defaultFile))
            {
                using (FileStream fileStream = new FileStream(defaultFile, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string rawString = reader.ReadToEnd();
                        //   var tempData = JsonConvert.DeserializeObject<List<ViewStepModel>>(rawString);
                        try
                        {
                            var tempDataLoad = JsonConvert.DeserializeObject<T>(rawString);
                            return tempDataLoad;
                        }
                        catch (Exception ex)
                        {


                        }

                    }
                }
            }
            return default;

        }
        public static void SaveJsonFile<T>(string fileName, string path, T dataSave)
        {
            string defaultFolder = path;
            if (string.IsNullOrEmpty(defaultFolder))
            {
                defaultFolder = _defaultFolder;
            }

            string defaultFile = $"{defaultFolder}\\{fileName}";
            if (!Directory.Exists(defaultFolder))
            {
                Directory.CreateDirectory(defaultFolder);
            }
            if (dataSave != null)
            {
                string filePath = defaultFile;
                string textResult = JsonConvert.SerializeObject(dataSave, Formatting.Indented);
                LogToFile(textResult, filePath, false);
            }


        }
        public static void SaveJsonFile<T>(string fileName, T dataSave)
        {
            string defaultFile = fileName;
            if (dataSave != null)
            {
                string filePath = defaultFile;
                string textResult = JsonConvert.SerializeObject(dataSave, Formatting.Indented);
                LogToFile(textResult, filePath, false);
            }
        }


        public static void DeleteFile(string fileName, string path)
        {
            string defaultFolder = path;
            if (string.IsNullOrEmpty(defaultFolder))
            {
                defaultFolder = _defaultFolder;
            }

            string defaultFile = $"{defaultFolder}\\{fileName}";
            if (File.Exists(defaultFile))
            {
                File.Delete(defaultFile);
            }

        }
        public static void LogToFile(string message, string fileName, bool bAppend)
        {
            try
            {
                lock (_lock)
                {
                    StreamWriter s1 = new StreamWriter($"{fileName}", bAppend);
                    s1.WriteLine(message);
                    s1.Flush();
                    s1.Close();

                }

            }
            catch (Exception ex)
            {

            }

        }      
    }
}
