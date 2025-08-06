using Newtonsoft.Json;
using System.IO;
using VSP_88D_CS.Models.Device;

namespace VSP_88D_CS.Common.Device
{
    public class MotionParametersRepository
    {
        string mtr_path_def = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA", "IO_DEFINE.json");

        public List<MotionParametersItem> Data { get; private set; } = new List<MotionParametersItem>();

        public MotionParametersRepository()
        {
            InitializeDefaultData();
        }
        public List<MotionParametersItem> GetAll()
        {
            try
            {
                return LoadFromFile(mtr_path_def);
            }
            catch (Exception ex)
            {

            }
            return new List<MotionParametersItem>();
        }

        private void InitializeDefaultData()
        {
            var existingData = GetAll().ToList();

            if (existingData.Count > 0)
            {
                Data = existingData;
                return;
            }
            else
            {
            }
            SaveToFile(mtr_path_def, Data);
        }

        public List<MotionParametersItem> LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<MotionParametersItem>();

            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<MotionParametersItem>>(json) ?? new List<MotionParametersItem>();
        }

        public void SaveToFile(string filePath, List<MotionParametersItem> data)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(filePath, json);
        }

        //public bool IsDataExist(MotionParametersItem motionParameters)
        //private bool Exists(MotionParametersItem motionParameters)
        //{
        //    return Data.Any(item => item.WireName == wireName);
        //}

        //public VSIOSettingItem GetIOSetting(int id)
        //{
        //    return Data.FirstOrDefault(x => x.Id == id);
        //}
        //public IEnumerable<VSIOSettingItem> GetAllIOSetting()
        //{
        //    return GetAll();
        //}
        //public async Task<bool> UpdateIOItem(VSIOSettingItem IOSettingItem)
        //{
        //    //return await _databaseManager.UpdateAsync<VSIOSettingItem>(IOSettingItem);
        //    throw new NotImplementedException();
        //}
        //public async Task<bool> DeleteIOItem(VSIOSettingItem IOSettingItem)
        //{
        //    //return await _databaseManager.DeleteAsync<VSIOSettingItem>(IOSettingItem);
        //    throw new NotImplementedException();
        //}
    }
}
