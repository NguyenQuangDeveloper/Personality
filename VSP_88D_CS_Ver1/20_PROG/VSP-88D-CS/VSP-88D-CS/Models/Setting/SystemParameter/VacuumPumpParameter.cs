using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting.SystemParameter
{
    public class VacuumPumpParameter : ViewModelBase
    {
        #region PROPERTY

        private bool _useOilSensor = true;
        public bool UseOilSensor
        {
            get => _useOilSensor;
            set => SetProperty(ref _useOilSensor, value);
        }

        private double _vacOffset = 0.00;
        public double VacOffset
        {
            get => _vacOffset;
            set => SetProperty(ref _vacOffset, value);
        }

        #endregion PROPERTY

        #region FUNCTION
        public VacuumPumpParameter() { }
        #endregion FUNCTION
    }
}
