using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting.SystemParameter
{
    public class EtcParameter : ViewModelBase
    {
        #region PROPERTY
        private bool _onTopFirst;
        public bool OnTopFirst
        {
            get => _onTopFirst;
            set => SetProperty(ref _onTopFirst, value);
        }

        private bool _outTopFirst;
        public bool OutTopFirst
        {
            get => _outTopFirst;
            set => SetProperty(ref _outTopFirst, value);
        }

        private bool _useSlotMatching;
        public bool UseSlotMatching
        {
            get => _useSlotMatching;
            set => SetProperty(ref _useSlotMatching, value);
        }

        private bool _inBufDirFwd;
        public bool InBufDirFwd
        {
            get => _inBufDirFwd;
            set => SetProperty(ref _inBufDirFwd, value);    
        }

        private bool _skipPlasma;
        public bool SkipPlasma
        {
            get => _skipPlasma;
            set => SetProperty(ref _skipPlasma, value);
        }

        private bool _dryRun;
        public bool DryRun
        {
            get => _dryRun;
            set => SetProperty(ref _dryRun, value);
        }
        private bool _useRfOverCheck;
        public bool UseRfOverCheck
        {
            get => _useRfOverCheck;
            set => SetProperty(ref _useRfOverCheck, value);
        }

        private bool _useN2Alarm;
        public bool UseN2Alarm
        {
            get => _useN2Alarm;
            set => SetProperty(ref _useN2Alarm, value);
        }

        private bool _useESim;
        public bool UseESim
        {
            get => _useESim;
            set => SetProperty(ref _useESim, value);
        }

        private int _gaugeValveOpenDelay;
        public int GaugeValveOpenDelay
        {
            get => _gaugeValveOpenDelay;
            set => SetProperty(ref _gaugeValveOpenDelay, value);
        }

        private int _inBufEmptyTime;
        public int InBufEmptyTime
        {
            get => _inBufEmptyTime;
            set => SetProperty(ref _inBufEmptyTime, value);
        }

        #endregion PROPERTY

        #region FUNCTION
        public EtcParameter() { }
        #endregion FUNCTION
    }
}
