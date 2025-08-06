using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting.SystemParameter
{
    public class VacuumGaugeParameter : ViewModelBase
    {
        #region PROPERTY

        private int _vacCal = 10;
        public int VacCal
        {
            get => _vacCal;
            set => SetProperty(ref _vacCal, value);
        }

        private double _vacErrAfterRfOn = 0.1;
        public double VacErrAfterRfOn
        {
            get => _vacErrAfterRfOn;
            set => SetProperty(ref _vacErrAfterRfOn, value);
        }

        #endregion PROPERY

        #region FUNCTION
        public VacuumGaugeParameter() { }

        #endregion FUNCTION
    }
}
