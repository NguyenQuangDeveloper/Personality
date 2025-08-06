using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using UserAccessLib.Common.Interfaces;
using UserAccessLib.Models;

namespace UserAccessLib.ViewModels
{
    public class LoginViewModel
    {
        private readonly IAuthService _authService;
        public LoginModel LoginModel { get; } = new();
        public ICommand LoginCommand { get; }

        public event Action<LoginInfo>? LoginSuccess;
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async () => await Login());

        }
        private async Task Login()
        {
            var user = await _authService.AuthenticateAsync(LoginModel.UserName, LoginModel.Password, LoginModel.RememberMe);
            if (user != null)
            {
                LoginSuccess?.Invoke(user);
            }
            else
            {
                LoginModel.ErrorMessage = "Login failed";
               // MessageBox.Show("Login failed");
            }
        }
    }
}
