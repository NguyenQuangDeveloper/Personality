using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting
{
    public class LeakCheckModel : ViewModelBase
    {
        public LeakCheckModel() { }

        private int _timeCount;
        public int TimeCount
        {
            get => _timeCount;
            set => SetProperty(ref _timeCount, value);  
        }

        private int _overPumpingTime;
        public int OverPumpingTime
        {
            get => _overPumpingTime;
            set => SetProperty(ref _overPumpingTime, value);
        }

        private int _stableTime;
        public int StableTime
        {
            get => _stableTime;
            set => SetProperty(ref _stableTime, value);
        }

        private int _leakCheckTime;
        public int LeakCheckTime
        {
            get => _leakCheckTime;
            set => SetProperty(ref _leakCheckTime, value);
        }
        private double _leakAlarmRate;
        public double LeakAlarmRate
        {
            get => _leakAlarmRate;
            set => SetProperty(ref _leakAlarmRate, value);
        }

        private DateTime _leakCheckStartTime;
        public DateTime LeakCheckStartTime
        {
            get => _leakCheckStartTime;
            set => SetProperty(ref _leakCheckStartTime, value);
        }

        private DateTime _leakCheckEndTime;
        public DateTime LeakCheckEndTime
        {
            get => _leakCheckEndTime;
            set => SetProperty(ref _leakCheckEndTime, value);
        }

        private int _startPressure;
        public int StartPressure
        {
            get => _startPressure;
            set => SetProperty(ref _startPressure, value);
        }
        private int _endPressure;
        public int EndPressure
        {
            get => _endPressure;
            set => SetProperty(ref _endPressure, value);
        }
        private double _leakRate;
        public double LeakRate
        {
            get => _leakRate;
            set => SetProperty(ref _leakRate, value);
        }
    }
}
