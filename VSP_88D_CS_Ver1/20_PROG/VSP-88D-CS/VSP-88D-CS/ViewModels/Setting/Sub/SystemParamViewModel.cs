using System.IO;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.ViewModels.Setting.Sub.SystemParameter;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class SystemParamViewModel : ViewModelBase
    {
        #region PROPERTY
        IGlobalSystemOption _globalSystemOption;
        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private MfcSystemParameterViewModel _mfcSystemParamViewModel;
        public MfcSystemParameterViewModel MfcSystemParamViewMode
        {
            get => _mfcSystemParamViewModel;
            set => SetProperty(ref _mfcSystemParamViewModel, value);
        }

        private RfGenSystemParameterViewModel _rfGenSystemParamViewModel;
        public RfGenSystemParameterViewModel RFGenSystemParamViewModel
        {
            get => _rfGenSystemParamViewModel;
            set => SetProperty(ref _rfGenSystemParamViewModel, value);
        }

        private VacGaugeSystemParameterViewModel _vacGaugeSystemParamViewModel;
        public VacGaugeSystemParameterViewModel VacGaugeSystemParamViewModel
        {
            get => _vacGaugeSystemParamViewModel;
            set => SetProperty(ref _vacGaugeSystemParamViewModel, value);
        }

        private VacPumpSystemParameterViewModel _vacPumpSystemParamViewModel;
        public VacPumpSystemParameterViewModel VacPumpSystemParamViewModel
        {
            get => _vacPumpSystemParamViewModel;
            set => SetProperty(ref _vacPumpSystemParamViewModel, value);
        }

        private EtcSystemParameterViewModel _etcSystemParamViewModel;

        public EtcSystemParameterViewModel EtcSystemParamViewModel
        {
            get => _etcSystemParamViewModel;
            set => SetProperty(ref _etcSystemParamViewModel, value);
        }

        #endregion PROPERTY

        #region FUNCTION

        public SystemParamViewModel()
        {
            _globalSystemOption = VSContainer.Instance.Resolve<IGlobalSystemOption>();
            _etcSystemParamViewModel = new EtcSystemParameterViewModel();
            _mfcSystemParamViewModel = new MfcSystemParameterViewModel();
            _rfGenSystemParamViewModel = new RfGenSystemParameterViewModel();
            _vacGaugeSystemParamViewModel = new VacGaugeSystemParameterViewModel();
            _vacPumpSystemParamViewModel = new VacPumpSystemParameterViewModel();

            FilePath = Path.Combine(_globalSystemOption.DataPath, "SYSTEM.JSON");

            SaveCommand = new RelayCommand(SaveParam);
            CancelCommand = new RelayCommand(LoadParam);
        }

        #endregion FUNCTION

        #region COMMAND
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        public void SaveParam()
        {
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                _mfcSystemParamViewModel.SaveParam(writer);
                _rfGenSystemParamViewModel.SaveParam(writer);
                _vacGaugeSystemParamViewModel.SaveParam(writer);
                _vacPumpSystemParamViewModel.SaveParam(writer);
                _etcSystemParamViewModel.SaveParam(writer);
            }
        }

        public void LoadParam()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }
            using (StreamReader reader = new StreamReader(FilePath))
            {
                int lineIndex = 0;
                string? line;

                while ((line = reader.ReadLine()) != null)
                {
                    var type = (eSystemOptType)lineIndex;
                    switch (type)
                    {
                        case eSystemOptType.MFC:
                            _mfcSystemParamViewModel.LoadParam(line);
                            break;
                        case eSystemOptType.RF_GEN:
                            _rfGenSystemParamViewModel.LoadParam(line);
                            break;
                        case eSystemOptType.VAC_GAUGE:
                            _vacGaugeSystemParamViewModel.LoadParam(line);
                            break;
                        case eSystemOptType.VAC_PUMP:
                            _vacPumpSystemParamViewModel.LoadParam(line);
                            break;
                        case eSystemOptType.ETC:
                            _etcSystemParamViewModel.LoadParam(line);
                            break;
                        default:
                            break;
                    }
                    lineIndex++;
                }
            }
        }

        #endregion EXECUTE COMMAND
    }
}
