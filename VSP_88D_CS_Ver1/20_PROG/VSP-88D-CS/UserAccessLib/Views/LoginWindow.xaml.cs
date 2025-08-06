using System.Windows;
using UserAccessLib.Common.Services;
using UserAccessLib.ViewModels;

namespace UserAccessLib.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginViewModel ViewModel { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            //ViewModel = new LoginViewModel(new AuthService());
            //DataContext = ViewModel;
            //ViewModel.LoginSuccess += ViewModel_LoginSuccess;

            DataContextChanged += LoginWindow_DataContextChanged;
        }
        private void LoginWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is LoginViewModel vm)
            {
                ViewModel = vm;
                vm.LoginSuccess += ViewModel_LoginSuccess;
            }
        }

        private void ViewModel_LoginSuccess(UserAccessLib.Models.LoginInfo obj)
        {
            this.DialogResult = true;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.LoginModel.Password = passwordBox.Password;
            }
        }
    }
}
