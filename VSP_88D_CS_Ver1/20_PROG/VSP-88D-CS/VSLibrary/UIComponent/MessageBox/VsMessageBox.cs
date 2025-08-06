using System.Windows;

namespace VSLibrary.UIComponent.MessageBox
{
    public static class VsMessageBox
    {
        private static class State
        {
            public static bool IsOpen = false;
            public static string? LastTitle;
            public static string? LastMessage;
            public static MessageBoxImage LastIcon;
            public static MessageBoxButton LastButtons;

            public static bool IsSame(string title, string message, MessageBoxImage icon, MessageBoxButton buttons) =>
                IsOpen &&
                LastTitle == title &&
                LastMessage == message &&
                LastIcon == icon &&
                LastButtons == buttons;
        }

        /// <summary>
        /// 현재 활성화된 WPF 윈도우를 반환합니다.
        /// </summary>
        private static Window? GetActiveWindow()
        {
            return Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive);
        }

        /// <summary>
        /// 사용자 정의 메시지 박스를 표시합니다.
        /// </summary>
        /// <param name="message">표시할 메시지 내용</param>
        /// <param name="title">창 제목 (기본값: "알림")</param>
        /// <param name="buttons">표시할 버튼 종류</param>
        /// <param name="icon">표시할 아이콘</param>
        /// <returns>사용자 선택 결과 (MessageBoxResult)</returns>
        public static MessageBoxResult Show(
            string message,
            string title = "알림",            
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult autoClick = MessageBoxResult.None, int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
        {           
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var msgBox = new VsMessageBoxWindow(title, message, icon, buttons, autoClick, autoClickDelaySeconds, enableDelaySeconds)
                {
                    Owner = GetActiveWindow()
                };
                msgBox.ShowDialog();
                return msgBox.Result; 
            });
        }

        public static void ShowAsync(string message, string title = "알림",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.None,
            Action<MessageBoxResult>? callback = null,
            MessageBoxResult autoClick = MessageBoxResult.None, int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
        {
            if (State.IsSame(title, message, icon, buttons))
                return;

            State.IsOpen = true;
            State.LastTitle = title;
            State.LastMessage = message;
            State.LastIcon = icon;
            State.LastButtons = buttons;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var msgBox = new VsMessageBoxWindow(title, message, icon, buttons, autoClick, autoClickDelaySeconds, enableDelaySeconds)
                    {
                        Owner = GetActiveWindow()
                    };
                    msgBox.ShowDialog();
                    callback?.Invoke(msgBox.Result);
                }
                finally
                {
                    State.IsOpen = false;
                    State.LastTitle = null;
                    State.LastMessage = null;
                }
            });
        }
    }
}
