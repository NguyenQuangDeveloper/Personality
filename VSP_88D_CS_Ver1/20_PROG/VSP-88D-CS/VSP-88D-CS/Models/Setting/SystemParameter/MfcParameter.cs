using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Setting.SystemParameter
{
    public class MfcParameter : ViewModelBase
    {
        #region PROPERTY

        // MFC 1
        private string _name1;
        public string Name1
        {
            get => _name1;
            set => SetProperty(ref _name1, value);   
        }
        private int _cal1;
        public int Cal1
        {
            get => _cal1;
            set => SetProperty(ref _cal1, value);
        }

        private int _rangeLow1;
        public int RangeLow1
        {
            get => _rangeLow1;
            set => SetProperty(ref _rangeLow1, value);
        }

        private int  _rangeHigh1;
        public int RangeHigh1
        {
            get => _rangeHigh1;
            set => SetProperty(ref _rangeHigh1, value);
        }

        private bool _use1;
        public bool Use1
        {
            get => _use1; 
            set => SetProperty(ref _use1, value);
        }

        // MFC 2
        private string _name2;
        public string Name2
        {
            get => _name2;
            set => SetProperty(ref _name2, value);
        }
        private int _cal2;
        public int Cal2
        {
            get => _cal2;
            set => SetProperty(ref _cal2, value);
        }

        private int _rangeLow2;
        public int RangeLow2
        {
            get => _rangeLow2;
            set => SetProperty(ref _rangeLow2, value);
        }

        private int _rangeHigh2;
        public int RangeHigh2
        {
            get => _rangeHigh2;
            set => SetProperty(ref _rangeHigh2, value);
        }

        private bool _use2;
        public bool Use2
        {
            get => _use2;
            set => SetProperty(ref _use2, value);
        }

        // MFC 3
        private string _name3;
        public string Name3
        {
            get => _name3;
            set => SetProperty(ref _name3, value);
        }
        private int _cal3;
        public int Cal3
        {
            get => _cal3;
            set => SetProperty(ref _cal3, value);
        }

        private int _rangeLow3;
        public int RangeLow3
        {
            get => _rangeLow3;
            set => SetProperty(ref _rangeLow3, value);
        }

        private int _rangeHigh3;
        public int RangeHigh3
        {
            get => _rangeHigh3;
            set => SetProperty(ref _rangeHigh3, value);
        }

        private bool _use3;
        public bool Use3
        {
            get => _use3;
            set => SetProperty(ref _use3, value);
        }

        // MFC 4
        private string _name4;
        public string Name4
        {
            get => _name4;
            set => SetProperty(ref _name4, value);
        }
        private int _cal4;
        public int Cal4
        {
            get => _cal4;
            set => SetProperty(ref _cal4, value);
        }

        private int _rangeLow4;
        public int RangeLow4
        {
            get => _rangeLow4;
            set => SetProperty(ref _rangeLow4, value);
        }

        private int _rangeHigh4;
        public int RangeHigh4
        {
            get => _rangeHigh4;
            set => SetProperty(ref _rangeHigh4, value);
        }

        private bool _use4;
        public bool Use4
        {
            get => _use4;
            set => SetProperty(ref _use4, value);
        }
        #endregion PROPERTY
    }

    public class MfcParameterList : ViewModelBase
    {
        private List<MfcParameter> _mfcParamList;
        public List<MfcParameter> MfcParamList
        {
            get => _mfcParamList;
            set => SetProperty(ref _mfcParamList, value);
        }

        public void Add( MfcParameter mfcParameter)
        {
            _mfcParamList.Add( mfcParameter );
        }
        public MfcParameterList()
        {

        }
    }
}
