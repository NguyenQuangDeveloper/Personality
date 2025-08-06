using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting.SystemParameter
{
    public class RfGenParameter : ViewModelBase
    {
        #region PROPERY
        private int _rfGenCal = 600;
        public int RfGenCal
        {
            get => _rfGenCal;
            set => SetProperty(ref _rfGenCal, value);
        }

        private int _rfPowerLimit = 550;
        public int RfPowerLimit
        {
            get => _rfPowerLimit;
            set =>  SetProperty(ref _rfPowerLimit, value);
        }

        private int _rfPowerOffset = 0;
        public int RfPowerOffset
        {
            get => _rfPowerOffset;
            set => SetProperty(ref _rfPowerOffset, value);
        }

        private int _rfPowerOverError = 5;
        public int RfPowerOverError
        {
            get => _rfPowerOverError;
            set => SetProperty(ref _rfPowerOverError, value);
        }

        #endregion PROPERTY

        #region FUNCTION
        public RfGenParameter() { }
        #endregion FUNCTION
    }
}
