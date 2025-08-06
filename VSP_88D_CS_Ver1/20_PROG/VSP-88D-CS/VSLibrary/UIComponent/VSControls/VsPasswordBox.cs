using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.VSControls;


/// <summary>
/// 힌트 텍스트를 지원하는 PasswordBox 확장 컨트롤입니다.
/// </summary>
public class VsPasswordBox : Control
{
    static VsPasswordBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(VsPasswordBox),
            new FrameworkPropertyMetadata(typeof(VsPasswordBox)));

        // 이미 같은 소스의 ResourceDictionary가 등록돼있는지 확인!
        var uri = new Uri("/VSLibrary;component/UIComponent/Styles/VsPasswordBoxStyle.xaml", UriKind.RelativeOrAbsolute);
        bool alreadyAdded = Application.Current.Resources.MergedDictionaries
            .OfType<ResourceDictionary>()
            .Any(x => x.Source != null && x.Source.Equals(uri));

        if (!alreadyAdded)
        {
            var dict = new ResourceDictionary { Source = uri };
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }

    public VsPasswordBox()
    {

    }

    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register(nameof(Hint), typeof(string), typeof(VsPasswordBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ErrorProperty =
        DependencyProperty.Register(nameof(Error), typeof(string), typeof(VsPasswordBox), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty IsPasswordEmptyProperty =
      DependencyProperty.Register(nameof(IsPasswordEmpty), typeof(bool), typeof(VsPasswordBox), new PropertyMetadata(true));

    public static readonly DependencyProperty PasswordProperty =
    DependencyProperty.Register(
        nameof(Password),
        typeof(string),
        typeof(VsPasswordBox),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public string Hint
    {
        get => (string)GetValue(HintProperty);
        set => SetValue(HintProperty, value);
    }

    public string Error
    {
        get => (string)GetValue(ErrorProperty);
        set => SetValue(ErrorProperty, value);
    }

    public bool IsPasswordEmpty
    {
        get => (bool)GetValue(IsPasswordEmptyProperty);
        set => SetValue(IsPasswordEmptyProperty, value);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_PasswordBox") is PasswordBox passwordBox)
        {
            // 🔹 초기 동기화: 외부 Password 값 → 내부 PasswordBox 반영
            if (passwordBox.Password != Password)
            {
                passwordBox.Password = Password ?? string.Empty;
            }

            passwordBox.PasswordChanged += (s, e) =>
            {
                // 입력 상태 동기화
                var pwd = passwordBox.Password;
                if (Password != pwd)
                    Password = pwd;

                IsPasswordEmpty = string.IsNullOrEmpty(pwd);
            };
            // 초기 상태 동기화
            IsPasswordEmpty = string.IsNullOrEmpty(passwordBox.Password);
        }
    }

    private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VsPasswordBox control && control.Password != null)
        {
            string newPassword = e.NewValue as string ?? string.Empty;

            // 이게 필요합니다!
            control.IsPasswordEmpty = string.IsNullOrEmpty(newPassword);

            // 템플릿 내부 PasswordBox도 동기화
            if (control.GetTemplateChild("PART_PasswordBox") is PasswordBox passwordBox)
            {
                if (passwordBox.Password != newPassword)
                    passwordBox.Password = newPassword;
            }
        }
    }
}