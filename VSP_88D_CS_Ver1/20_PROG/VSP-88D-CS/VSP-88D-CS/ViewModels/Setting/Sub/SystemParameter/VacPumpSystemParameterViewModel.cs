using System.IO;
using System.Text.Json;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting.SystemParameter;

namespace VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter
{
    public class VacPumpSystemParameterViewModel : ViewModelBase
    {
        #region PROPERY
        private VacuumPumpParameter _vacPumpParam;
        public VacuumPumpParameter VacPumpParam
        {
            get => _vacPumpParam;
            set => SetProperty(ref _vacPumpParam, value);
        }
        #endregion PROPERY

        #region FUNCTION
        public VacPumpSystemParameterViewModel()
        {
            IGlobalSystemOption globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _vacPumpParam = globalSystemOption.VacPumpParam;
        }
        public void SaveParam(StreamWriter writer)
        {
            if (null == writer)
                return;
            string json = JsonSerializer.Serialize(_vacPumpParam);
            writer.WriteLine($"{json}");
        }

        public void LoadParam(string line)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return;
            var param = JsonSerializer.Deserialize<VacuumPumpParameter>(line);
            if (null != param)
                _vacPumpParam = param;
        }
        #endregion FUNCTION
    }
}
