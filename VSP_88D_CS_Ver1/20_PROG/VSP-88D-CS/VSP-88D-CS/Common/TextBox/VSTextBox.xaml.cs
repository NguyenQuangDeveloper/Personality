using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace VSP_88D_CS.Common.TextBox
{
    public partial class VSTextBox : UserControl
    {
        public static readonly DependencyProperty IsEnterPressedProperty =
            DependencyProperty.Register("IsEnterPressed", typeof(bool), typeof(VSTextBox),
        new PropertyMetadata(false));

        public bool IsEnterPressed
        {
            get => (bool)GetValue(IsEnterPressedProperty);
            set => SetValue(IsEnterPressedProperty, value);
        }
        // OffsetX DependencyProperty
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(VSTextBox), new PropertyMetadata(0.0));

        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        // OffsetY DependencyProperty
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(VSTextBox), new PropertyMetadata(0.0));

        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        // IsFocused DependencyProperty
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register("IsFocused", typeof(bool), typeof(VSTextBox),
                new PropertyMetadata(false, OnIsFocusedChanged));

        public bool IsFocused
        {
            get => (bool)GetValue(IsFocusedProperty);
            set => SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VSTextBox control && control.PART_TextBoxContainer.Content is FrameworkElement textBox)
            {
                if ((bool)e.NewValue)
                {
                    // 포커스 설정
                    textBox.Focus();
                }
                else
                {
                    // 포커스 해제
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(textBox), null);
                }
            }
        }

        // Title DependencyProperty
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(VSTextBox), new PropertyMetadata("Titel"));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // MinValue DependencyProperty
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(string), typeof(VSTextBox), new PropertyMetadata("0"));

        public string MinValue
        {
            get => (string)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        // MaxValue DependencyProperty
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(string), typeof(VSTextBox), new PropertyMetadata("9999"));

        public string MaxValue
        {
            get => (string)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        // Text DependencyProperty
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VSTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // IsOnlyNumber DependencyProperty
        public static readonly DependencyProperty IsOnlyNumberProperty =
            DependencyProperty.Register("IsOnlyNumber", typeof(bool), typeof(VSTextBox),
                new PropertyMetadata(false));

        public bool IsOnlyNumber
        {
            get => (bool)GetValue(IsOnlyNumberProperty);
            set => SetValue(IsOnlyNumberProperty, value);
        }

        // IsPassword DependencyProperty
        public static readonly DependencyProperty IsPasswordProperty =
            DependencyProperty.Register("IsPassword", typeof(bool), typeof(VSTextBox),
                new PropertyMetadata(false, OnIsPasswordChanged));

        public bool IsPassword
        {
            get => (bool)GetValue(IsPasswordProperty);
            set => SetValue(IsPasswordProperty, value);
        }

        public VSTextBox()
        {
            InitializeComponent();
            UpdateControlTemplate();
            this.KeyDown += OnKeyDown;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsEnterPressed = true;
                // 추가 동작이 필요하다면 여기서 처리
            }
            else
            {
                IsEnterPressed = false;
            }
        }
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VSTextBox control)
            {
                control.UpdateText();
                // PasswordBoxHelper의 BoundPassword 업데이트
                if (control.PART_TextBoxContainer.Content is PasswordBox passwordBox)
                {
                    //PasswordBoxHelper.SetBoundPassword(passwordBox, control.Text); aaa
                }
            }
        }

        private static void OnIsPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VSTextBox control)
            {
                control.UpdateControlTemplate();
            }
        }

        private void UpdateControlTemplate()
        {
            if (PART_TextBoxContainer == null) return;

            // 기존 컨트롤 제거
            PART_TextBoxContainer.Content = null;

            if (IsPassword)
            {
                // PasswordBox로 전환
                var passwordBox = new PasswordBox
                {
                    Password = Text
                };

                // 이벤트 연결
                // PasswordChanged 이벤트 연동
                passwordBox.PasswordChanged += (s, e) =>
                {
                    //aaa
                    //if (!(bool)passwordBox.GetValue(PasswordBoxHelper.UpdatingPasswordProperty))
                    //{
                    //    passwordBox.SetValue(PasswordBoxHelper.UpdatingPasswordProperty, true);
                    //    Text = passwordBox.Password; // Text 속성 동기화
                    //    PasswordBoxHelper.SetBoundPassword(passwordBox, passwordBox.Password); // BoundPassword 업데이트
                    //    passwordBox.SetValue(PasswordBoxHelper.UpdatingPasswordProperty, false);
                    //}
                };

                passwordBox.PreviewTextInput += PART_TextBox_PreviewTextInput;
                passwordBox.GotFocus += PART_TextBox_GotFocus;
                passwordBox.LostFocus += PART_TextBox_LostFocus;

                PART_TextBoxContainer.Content = passwordBox;
            }
            else
            {
                // TextBox로 전환
                var textBox = new System.Windows.Controls.TextBox
                {
                    Text = Text
                };

                // 이벤트 연결
                textBox.TextChanged += (s, e) => Text = textBox.Text;
                textBox.PreviewTextInput += PART_TextBox_PreviewTextInput;
                textBox.GotFocus += PART_TextBox_GotFocus;
                textBox.LostFocus += PART_TextBox_LostFocus;

                PART_TextBoxContainer.Content = textBox;
            }
        }

        private void UpdateText()
        {
            if (PART_TextBoxContainer.Content is System.Windows.Controls.TextBox textBox)
            {
                textBox.Text = Text;
            }
            else if (PART_TextBoxContainer.Content is PasswordBox passwordBox)
            {
                passwordBox.Password = Text;
            }
        }

        private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsOnlyNumber)
            {
                var input = e.Text;
                e.Handled = !Regex.IsMatch(input, "^[0-9.]*$");
            }
        }
        private Window _numberKeyDialog; // 숫자 키패드 창 저장 필드
        private void PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (IsOnlyNumber)
                {
                    // 숫자 키패드 열기
                    OpenNumberKeyPad(sender);
                }
                else
                {
                    // 기본 키패드 열기
                    OpenDefaultKeyboard(sender);
                }
            });
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsOnlyNumber) // IsOnlyNumber가 true인 경우 필터링
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (_numberKeyDialog != null)
                    {
                        _numberKeyDialog.Close(); // 창 닫기
                        _numberKeyDialog = null;
                    }
                });
            }
            // PasswordBox의 경우 PasswordBoxHelper의 UpdatingPasswordProperty를 초기화
            if (PART_TextBoxContainer.Content is PasswordBox passwordBox)
            {
                // aaa
                //passwordBox.SetValue(PasswordBoxHelper.UpdatingPasswordProperty, false); // 초기화
                //PasswordBoxHelper.SetBoundPassword(passwordBox, passwordBox.Password); // BoundPassword 업데이트
            }
        }
        private void OpenNumberKeyPad(object sender)
        {
            // aa
            //if (sender is FrameworkElement element)
            //{
            //    // aaa
            //    //var numberKey = new NumberKey
            //    //{
            //    //    MinValue = this.MinValue, // MinValue 전달
            //    //    MaxValue = this.MaxValue  // MaxValue 전달
            //    //};

            //    _numberKeyDialog = new Window
            //    {
            //        Content = numberKey,
            //        Width = 240,
            //        Height = 240,
            //        WindowStyle = WindowStyle.ToolWindow,
            //        ResizeMode = ResizeMode.NoResize,
            //        Owner = Application.Current.MainWindow,
            //        Topmost = true,
            //        Title = Title
            //    };

            //    if (!string.IsNullOrWhiteSpace(Text) && long.TryParse(Text, out long nValue))
            //    {
            //        numberKey.InputBox.Text = nValue.ToString();
            //    }

            //    // 현재 TextBox의 화면 좌표 가져오기
            //    var point = element.PointToScreen(new System.Windows.Point(0, element.ActualHeight));
            //    _numberKeyDialog.Left = point.X + OffsetX; // X 좌표에 OffsetX 추가
            //    _numberKeyDialog.Top = point.Y + OffsetY; // Y 좌표에 OffsetY 추가

            //    numberKey.DialogResult += (s, result) =>
            //    {
            //        if (result)
            //        {
            //            Text = numberKey.InputValue; // 입력값 설정
            //        }
            //        _numberKeyDialog.Close();
            //        _numberKeyDialog = null;
            //    };

            //    _numberKeyDialog.ShowDialog();
            //}
        }

        private void OpenDefaultKeyboard(object sender)
        {
            // aaa
            //if (sender is FrameworkElement element)
            //{
            //    var keyboard = new VSKeyboard();
            //    _numberKeyDialog = new Window
            //    {
            //        Content = keyboard,
            //        Width = 880,
            //        Height = 338,
            //        WindowStyle = WindowStyle.ToolWindow,
            //        ResizeMode = ResizeMode.NoResize,
            //        Owner = Application.Current.MainWindow,
            //        Topmost = true
            //    };
            //    keyboard.EnterPressed += value =>
            //    {
            //        Text = value; // 입력된 값을 Text 속성에 설정
            //        IsEnterPressed = true; // EnterPressed 상태 설정
            //    };

            //    keyboard.InputBox.Text = Text;
               

            //    // 현재 TextBox의 화면 좌표 가져오기
            //    var point = element.PointToScreen(new System.Windows.Point(0, element.ActualHeight));
            //    _numberKeyDialog.Left = point.X + OffsetX; // X 좌표에 OffsetX 추가
            //    _numberKeyDialog.Top = point.Y + OffsetY; // Y 좌표에 OffsetY 추가

            //    keyboard.InputBox.TextChanged += (s, e) =>
            //    {
            //        Text = keyboard.InputBox.Text;
            //    };

            //    _numberKeyDialog.ShowDialog();
            //}
        }
    }
}
