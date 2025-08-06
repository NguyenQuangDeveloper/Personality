using System;
using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.MessageBox
{
    /// <summary>
    /// 사용자 정의 메시지 박스 윈도우입니다.
    /// 다양한 버튼과 아이콘 타입을 지원합니다.
    /// </summary>
    public partial class VsMessageBoxWindow : Window
    {
        /// <summary>
        /// 사용자 결과 반환값
        /// </summary>
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        /// <summary>
        /// 생성자 (내부 사용)
        /// </summary>
        public VsMessageBoxWindow(string title, string message, MessageBoxImage icon, MessageBoxButton buttons, MessageBoxResult autoClickTarget = MessageBoxResult.None, int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
        {
            InitializeComponent();

            TxtTitle.Text = title;
            TxtMessage.Text = message;
            TxtIcon.Text = GetEmoji(icon);

            // 버튼 구성
            SetupButtons(buttons);

            if (enableDelaySeconds > 0)
                DisableButtonsAndEnableLater(enableDelaySeconds);

            if (autoClickTarget != MessageBoxResult.None && autoClickDelaySeconds > 0)
                StartAutoClick(autoClickTarget, autoClickDelaySeconds);
        }

        /// <summary>
        /// 버튼을 잠시 비활성화한 후, 설정된 시간 이후에 다시 활성화합니다.
        /// 버튼 텍스트에 카운트다운도 표시됩니다.
        /// </summary>
        private async void DisableButtonsAndEnableLater(int delaySeconds)
        {
            var buttons = new[]{ BtnOk, BtnYes, BtnNo, BtnCancel};

            // 버튼별 원래 텍스트 저장
            var originalTexts = buttons.ToDictionary(btn => btn, btn => btn.Content?.ToString());

            foreach (var btn in buttons)
                btn.IsEnabled = false;

            for (int i = delaySeconds; i > 0; i--)
            {
                foreach (var btn in buttons.Where(b => b.Visibility == Visibility.Visible))
                    btn.Content = $"{originalTexts[btn]} ({i})";

                await Task.Delay(1000);
            }

            foreach (var btn in buttons)
            {
                if (btn.Visibility == Visibility.Visible)
                {
                    btn.IsEnabled = true;
                    btn.Content = originalTexts[btn];
                }
            }
        }

        /// <summary>
        /// 지정된 시간 후에 특정 버튼을 자동 클릭하며, 버튼 텍스트에 카운트다운을 표시합니다.
        /// </summary>
        private async void StartAutoClick(MessageBoxResult target, int delaySeconds)
        {
            var button = GetTargetButton(target);
            if (button == null) return;

            string originalText = button.Content?.ToString() ?? "";

            for (int i = delaySeconds; i > 0; i--)
            {
                button.Content = $"{originalText} ({i})";
                await Task.Delay(1000);
            }

            button.Content = originalText;

            switch (target)
            {
                case MessageBoxResult.OK: BtnOk_Click(null!, null!); break;
                case MessageBoxResult.Yes: BtnYes_Click(null!, null!); break;
                case MessageBoxResult.No: BtnNo_Click(null!, null!); break;
                case MessageBoxResult.Cancel: BtnCancel_Click(null!, null!); break;
            }
        }

        /// <summary>
        /// MessageBoxResult 값에 대응하는 버튼을 반환합니다.
        /// </summary>
        private Button? GetTargetButton(MessageBoxResult result) => result switch
        {
            MessageBoxResult.OK => BtnOk,
            MessageBoxResult.Cancel => BtnCancel,
            MessageBoxResult.Yes => BtnYes,
            MessageBoxResult.No => BtnNo,
            _ => null
        };

        /// <summary>
        /// MessageBoxImage에 따라 아이콘 이모지를 반환합니다.
        /// </summary>
        private string GetEmoji(MessageBoxImage icon)
        {
            return icon switch
            {
                MessageBoxImage.Error => "❌",
                MessageBoxImage.Warning => "⚠️",
                MessageBoxImage.Information => "ℹ️",
                MessageBoxImage.Question => "❓",
                _ => "🔔"
            };
        }

        /// <summary>
        /// 버튼 구성
        /// </summary>
        private void SetupButtons(MessageBoxButton buttons)
        {
            BtnOk.Visibility = Visibility.Collapsed;
            BtnYes.Visibility = Visibility.Collapsed;
            BtnNo.Visibility = Visibility.Collapsed;
            BtnCancel.Visibility = Visibility.Collapsed;

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    BtnOk.Focus();
                    BtnOk.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    BtnOk.Focus();
                    BtnOk.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    BtnYes.Focus();
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    BtnYes.Focus();
                    BtnYes.Visibility = Visibility.Visible;
                    BtnNo.Visibility = Visibility.Visible;
                    BtnCancel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }
    }
}
