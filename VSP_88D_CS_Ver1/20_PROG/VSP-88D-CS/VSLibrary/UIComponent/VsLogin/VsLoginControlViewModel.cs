using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows;
using VSLibrary.Common.Log;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.MessageBox;
using VSLibrary.UIComponent.VsLogin.Repository;

namespace VSLibrary.UIComponent.VsLogin;

public partial class VsLoginControlViewModel : ViewModelBase
{
    [ObservableProperty]
    public string _id = string.Empty;

    [ObservableProperty]
    public string _idError = string.Empty;

    [ObservableProperty]
    public string _password = string.Empty;

    [ObservableProperty]
    public string _passwordError = string.Empty;

    // 로그인 상태 변경 알림 (로그인/로그아웃 시 발생)
    public event Action<UserItem?>? LoginChanged;

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            VsMessageBox.ShowAsync("아이디를 입력하세요.", "로그인 오류", autoClick: MessageBoxResult.OK, autoClickDelaySeconds: 3);
            IdError = "아이디를 입력하세요.";
            return;
        }
        IdError = string.Empty;
        if (string.IsNullOrWhiteSpace(Password))
        {
            VsMessageBox.ShowAsync("비밀번호를 입력하세요.", "로그인 오류", autoClick: MessageBoxResult.OK, autoClickDelaySeconds: 3);
            PasswordError = "비밀번호를 입력하세요.";
            return;
        }

        PasswordError = IdError = string.Empty;

        bool isValid = await LoginRepository.Instance.ValidateAsync(Id, Password);

        if (isValid)
        {
            var user = LoginRepository.Instance.FindUser(Id);
            LogManager.Write($"✅ Login success: ID = {user?.UserID}, Grade = {user?.Grade}", LogType.Info);
            LoginChanged?.Invoke(user); 
            await Close();
        }
        else
        {
            LogManager.Write($"❌ Login failed: ID = {Id}", LogType.Warn);
            IdError = "아이디 또는 비밀번호가 일치하지 않습니다.";
            PasswordError = "아이디 또는 비밀번호가 일치하지 않습니다.";
        }
    }

    public static bool IsShow;

    /// <summary>
    /// 로그인 다이얼로그를 호출하고, 로그인 성공 시 UserItem 반환.
    /// 실패 또는 취소 시 null 반환.
    /// </summary>
    public static async Task<UserItem?> ShowLoginDialogAsync()
    {
        if (IsShow) return null; // 이미 로그인 창이 열려있으면 중복 호출 방지
        IsShow = true;
        var tcs = new TaskCompletionSource<UserItem?>();

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Content is FrameworkElement root &&
                    root.DataContext is VsLoginControlViewModel vm)
                {
                    vm.LoginChanged += OnLoginCompleted;
                    window.Show();
                    window.Activate();                   
                    return;
                }
            }

            LogManager.Write("⚠️ 로그인 창을 찾을 수 없습니다. VsLoginControl이 Window에 바인딩되어 있어야 합니다.", LogType.Warn);
            tcs.TrySetResult(null);
        });

     
        return await tcs.Task;

        void OnLoginCompleted(UserItem? user)
        {
            if (!tcs.Task.IsCompleted)
                tcs.TrySetResult(user);
        }
    }

    [RelayCommand]
    private async Task Close()
    {
        Id = string.Empty;
        IdError = string.Empty;
        Password = ""; 
        PasswordError = string.Empty;

        await Task.Delay(100);
        // 부모 윈도우 닫기 시도
        foreach (Window window in Application.Current.Windows)
        {
            if (window.Content is FrameworkElement root && root.DataContext == this)
            {
                IsShow = false;
                LoginChanged?.Invoke(null);
                window.Hide();
                break;
            }
        }
    }    
}
