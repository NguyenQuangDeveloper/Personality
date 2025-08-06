using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Views.Setting.Sub;

namespace VSP_88D_CS.Models.Recipe
{
    public class DeviceData : ViewModelBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public int EntityID { get; set; }
        //public string Id { get; set; }
        public string DeviceName { get; set; }
        public List<MotionData> MotionItems { get; set; }
        public DeviceData()
        {
            DeviceName = string.Empty;
            MotionItems = new List<MotionData>();
        }
        public DeviceData Clone()
        {
             //var motionItems = MotionItems
             //.Select(m => new MotionData
             //{
             //     MotionName=m.MotionName,
             //     IsLeftRight=m.IsLeftRight,
             //     IsUpDown=m.IsUpDown,
             //     MotionParameters = new List<MotionParameter>(m.MotionParameters) 
       
             //}).ToList();
            var motionItems = MotionItems
             .Select(m => new MotionData
             {
                 MotionName = m.MotionName,
                 IsLeftRight = m.IsLeftRight,
                 IsUpDown = m.IsUpDown,
                 ServoState= m.ServoState,
                 MotionParameters = m.MotionParameters
                 .Select(x=> new MotionParameter {
                     Acceleration=x.Acceleration,
                     Index=x.Index,
                     Position=x.Position,
                     Description=x.Description,
                     Velocity=x.Velocity
                 }).ToList()

             }).ToList();
            var deviceData= new DeviceData
            {
                EntityID = this.EntityID,
                DeviceName = this.DeviceName,
                MotionItems = new List<MotionData>(motionItems)

            };
            return deviceData;
        }

        public DeviceData(DeviceItem deviceItem)
        {
            this.EntityID = deviceItem.Id;
            this.DeviceName = deviceItem.Name;
        }

        public DeviceItem ToEntity()
        {
            return new()
            {
                Id = this.EntityID,
                Name = this.DeviceName,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
