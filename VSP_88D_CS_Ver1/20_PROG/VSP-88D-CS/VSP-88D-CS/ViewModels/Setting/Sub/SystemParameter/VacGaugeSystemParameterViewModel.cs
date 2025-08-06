using System.IO;
using System.Text.Json;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting.SystemParameter;

namespace VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter
{
    public class VacGaugeSystemParameterViewModel : ViewModelBase
    {
        #region PROPERTY
        private VacuumGaugeParameter _vacGaugeParam;
        public VacuumGaugeParameter VacGaugeParam
        {
            get => _vacGaugeParam;
            set => SetProperty(ref _vacGaugeParam, value);
        }
        #endregion PROPERTY

        #region FUNCTION
        public VacGaugeSystemParameterViewModel() 
        {
            IGlobalSystemOption globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _vacGaugeParam = globalSystemOption.VacGaugeParam;  
        }
        public void SaveParam(StreamWriter writer)
        {
            if (null == writer)
                return;
            string json = JsonSerializer.Serialize(_vacGaugeParam);
            writer.WriteLine($"{json}");
        }

        public void LoadParam(string line)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return;
            var param = JsonSerializer.Deserialize<VacuumGaugeParameter>(line);
            if(null != param)
                _vacGaugeParam = param;
        }
        #endregion FUNCTION
    }
}
