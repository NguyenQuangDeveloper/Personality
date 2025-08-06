using System.Diagnostics;
using System.IO;
using System.Windows;
using UserAccessLib.Common.Interfaces;
using UserAccessLib.Models;
using UserAccessLib.ViewModels;
using UserAccessLib.Views;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Setting;


namespace VSP_88D_CS.Common.Auth
{
    public class AuthFunctions
    {
        private static LoginInfo _currentUser;

        public static LoginInfo CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; }
        }


        private static void ShowLogin(IAuthService authService)
        {
            var loginVM = new LoginViewModel(authService);
            loginVM.LoginSuccess += OnLoginSuccess;
            var loginView = new LoginWindow
            {
                DataContext = loginVM
            };
            loginView.ShowDialog();

        }
        private static void OnLoginSuccess(LoginInfo loginInfo)
        {
            _currentUser = loginInfo;
        }

        public static bool HavePermission(IAuthService authService, eFunctionItem functionItem, IGlobalSystemOption globalSystemOption)
        {
            #if DEBUG
            Debug.WriteLine("Login ByPass when Debugging");
            return true;           
            #endif

          
            if (_currentUser == null)
            {
                ShowLogin(authService);
            }
            if (_currentUser == null)
            {
                MessageUtils.ShowWarning("Login First");
                return false;
            }
            string fileMatrix = $"{globalSystemOption.DataPath}/Permission.mtx";
            FunctionPermission item;
            bool result = false;
            if (File.Exists(fileMatrix))
            {
                var listFunctionPermission = SupportFunctions.LoadJsonFile<List<FunctionPermission>>(fileMatrix);
                item = listFunctionPermission.FirstOrDefault(x => x.KeyItem == functionItem);
                switch (_currentUser.UserRole)
                {
                    case UserAccessLib.Common.Enum.UserRole.Maker:
                        result = true;
                        break;
                    case UserAccessLib.Common.Enum.UserRole.Admin:
                        result = item.Level3;
                        break;
                    case UserAccessLib.Common.Enum.UserRole.Manager:
                        result = item.Level2;
                        break;
                    case UserAccessLib.Common.Enum.UserRole.Operator:
                        result = item.Level1;
                        break;

                    default:
                        result = item.Level1;
                        break;
                }
                if (item != null && item.AllowAll) { result = true; }
            }
            else
            {
                if (_currentUser.UserRole == UserAccessLib.Common.Enum.UserRole.Maker) result = true;
            }
            if (!result)
            {
                MessageBox.Show("No Permission", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            if (!_currentUser.RememberMe)
            {
                _currentUser = null;

            }

            return result;
        }


    }
}
