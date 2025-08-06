using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting.SystemParameter;

namespace VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter
{
    public class EtcSystemParameterViewModel : ViewModelBase
    {
        #region PROPERTY
        private EtcParameter _etcParam = new();
        public EtcParameter EtcParam
        {
            get => _etcParam;
            set => SetProperty(ref _etcParam, value);
        }

        #endregion PROPERTY

        #region FUCTION
        public EtcSystemParameterViewModel() 
        {
            IGlobalSystemOption globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _etcParam = globalSystemOption.EtcParam;
        }

        public void SaveParam(StreamWriter writer)
        {
            if (null == writer)
                return;
            string json = JsonSerializer.Serialize(_etcParam);
            writer.WriteLine($"{json}");
        }

        public void LoadParam(string line)
        {
            line = line.Trim();
            if(string.IsNullOrWhiteSpace(line))
                return;
            var param = JsonSerializer.Deserialize<EtcParameter>(line);
            if(null != param)
                _etcParam = param;
        }

        #endregion FUNCTION

    }
}
