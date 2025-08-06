using System.IO;
using System.Text.Json;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting.SystemParameter;

namespace VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter
{
    public class MfcSystemParameterViewModel: ViewModelBase
    {
        #region PROPERTY
        private MfcParameter _mfcParam;
        public MfcParameter MfcParam
        {
            get => _mfcParam;
            set => SetProperty(ref _mfcParam, value);   
        }
        #endregion PROPERYY

        #region FUNCTION
        public MfcSystemParameterViewModel() 
        {
            IGlobalSystemOption globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _mfcParam = globalSystemOption.MfcParam;
        }

        public void SaveParam(StreamWriter writer)
        {
            if (null == writer)
                return;
            string json = JsonSerializer.Serialize(_mfcParam);
            writer.WriteLine($"{json}");
        }

        public void LoadParam(string line)
        {
            line = line.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return;
            var param = JsonSerializer.Deserialize<MfcParameter>(line);
            if(null != param)
                _mfcParam = param;
        }
        #endregion FUNCTION
    }
}
