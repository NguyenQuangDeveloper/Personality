using System.Windows;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class SvPvViewModel : ViewModelBase
    {
        //private bool Use4Mfc;
        public LanguageService LanguageResources { get; }
        #region PROPERTY
        private string _stepSetValue;
        public string StepSetValue
        {
            get => _stepSetValue;
            set => SetProperty(ref _stepSetValue, value);
        }
        private string _stepCurrentValue;
        public string StepCurrentValue
        {
            get => _stepCurrentValue;
            set => SetProperty(ref _stepCurrentValue, value);
        }

        private string _rfSetValue;
        public string RFSetValue
        {
            get => _rfSetValue;
            set => SetProperty(ref _rfSetValue, value);
        }
        private string _rfCurrentValue;
        public string RFCurrentValue
        {
            get => _rfCurrentValue;
            set => SetProperty(ref _rfCurrentValue, value);
        }
        private string _vacuumSetValue;
        public string VacuumSetValue
        {
            get => _vacuumSetValue;
            set => SetProperty(ref _vacuumSetValue, value);
        }
        private string _vacuumCurrentValue;
        public string VacuumCurrentValue
        {
            get => _vacuumCurrentValue;
            set => SetProperty(ref _vacuumCurrentValue, value);
        }
        private string _gas1SetValue;
        public string Gas1SetValue
        {
            get => _gas1SetValue;
            set => SetProperty(ref _gas1SetValue, value);
        }
        private string _gas1CurrentValue;
        public string Gas1CurrentValue
        {
            get => _gas1CurrentValue;
            set => SetProperty(ref _gas1CurrentValue, value);
        }
        private string _gas2SetValue;
        public string Gas2SetValue
        {
            get => _gas2SetValue;
            set => SetProperty(ref _gas2SetValue, value);
        }
        private string _gas2CurrentValue;
        public string Gas2CurrentValue
        {
            get => _gas2CurrentValue;
            set => SetProperty(ref _gas2CurrentValue, value);
        }
        //private string _gas3SetValue;
        //public string Gas3SetValue
        //{
        //    get => _gas3SetValue;
        //    set => SetProperty(ref _gas3SetValue, value);
        //}
        //private string _gas4SetValue;
        //public string Gas4SetValue
        //{
        //    get => _gas4SetValue;
        //    set => SetProperty(ref _gas4SetValue, value);
        //}

        private string _cleaningSetValue;
        public string CleaningSetValue
        {
            get => _cleaningSetValue;
            set => SetProperty(ref _cleaningSetValue, value);
        }
        private string _cleaningCurrentValue;
        public string CleaningCurrentValue
        {
            get => _cleaningCurrentValue;
            set => SetProperty(ref _cleaningCurrentValue, value);
        }
        //private string _gas3CurrentValue;
        //public string Gas3CurrentValue
        //{
        //    get => _gas3CurrentValue;
        //    set => SetProperty(ref _gas3CurrentValue, value);
        //}
        //private string _gas4CurrentValue;
        //public string Gas4CurrentValue
        //{
        //    get => _gas4CurrentValue;
        //    set => SetProperty(ref _gas4CurrentValue, value);
        //}
        private GridLength _rowHeight;
        public GridLength RowHeight
        {
            get => _rowHeight;
            set => SetProperty(ref _rowHeight, value);
        }
        #endregion PROPERTY
        public SvPvViewModel()
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            //TODO check option Use4Mfc for display GAS3 GAS4
            //Use4Mfc = true;
            //if (Use4Mfc)
            //{
            //    RowHeight = new GridLength(1, GridUnitType.Star);
            //}
            //else RowHeight = new GridLength(0);
        }
    }
}
