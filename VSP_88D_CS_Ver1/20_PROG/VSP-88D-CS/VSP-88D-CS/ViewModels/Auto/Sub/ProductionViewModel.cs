using System.Windows.Input;
using UserAccessLib.Common.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Auth;
using VSP_88D_CS.Views.Auto.Sub;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class ProductionViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IGlobalSystemOption _globalSystemOption;

        private Initial _initial;
        #region PROPERTY
        public LanguageService LanguageResources { get; }
        private int _totalProduct;
        public int TotalProduct
        {
            get => _totalProduct;
            set => SetProperty(ref _totalProduct, value);
        }

        private int _lotCount;
        public int LotCount
        {
            get => _lotCount;
            set => SetProperty(ref _lotCount, value);
        }

        private double _cycleSec;
        public double CycleSec
        {
            get => _cycleSec;
            set => SetProperty(ref _cycleSec, value);
        }

        private double _productsPerHour;
        public double ProductsPerHour
        {
            get => _productsPerHour;
            set => SetProperty(ref _productsPerHour, value);
        }

        #endregion PROPERTY
        #region COMMAND
        public ICommand InitialShowCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        private void InitialShow()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.InitialFunction, _globalSystemOption))
            {
                return;
            }
            _initial.ShowDialog();
        }
        private void Reset()
        {
            if (!AuthFunctions.HavePermission(_authService, Models.Setting.eFunctionItem.ResetFunction, _globalSystemOption))
            {
                return;
            }

        }
        #endregion EXECUTE COMMAND

        public ProductionViewModel(Initial initial, IGlobalSystemOption globalSystemOption, IAuthService authService)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _initial = initial;

            InitialShowCommand = new RelayCommand(InitialShow);
            ResetCommand = new RelayCommand(Reset);

            //TEST value
            TotalProduct = 9999999;
            LotCount = 99999;
            CycleSec = 999;
            ProductsPerHour = 9999;
            _globalSystemOption = globalSystemOption;
            _authService = authService;

        }
    }
}
