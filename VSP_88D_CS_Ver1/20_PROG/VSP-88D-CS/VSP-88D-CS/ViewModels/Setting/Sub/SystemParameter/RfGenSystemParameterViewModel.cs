using System.IO;
using System.Text.Json;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting.SystemParameter;

namespace VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter
{
    public class RfGenSystemParameterViewModel : ViewModelBase
    {
        #region PROPERTY
        private RfGenParameter _rfGenParam;
        public RfGenParameter RfGenParam
        {
            get => _rfGenParam;
            set => SetProperty(ref _rfGenParam, value);
        }

        #endregion PROPERTY

        #region FUNCTION
        public RfGenSystemParameterViewModel()
        {
            IGlobalSystemOption globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _rfGenParam = globalSystemOption.RfGenParam;
        }
        public void SaveParam(StreamWriter writer)
        {
            if (null == writer)
                return;
            string json = JsonSerializer.Serialize(_rfGenParam);
            writer.WriteLine($"{json}");
        }

        public void LoadParam(string line)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return;
            var param = JsonSerializer.Deserialize<RfGenParameter>(line);
            if(null != param)
                _rfGenParam = param;
        }
        #endregion FUNCTION
    }
}
