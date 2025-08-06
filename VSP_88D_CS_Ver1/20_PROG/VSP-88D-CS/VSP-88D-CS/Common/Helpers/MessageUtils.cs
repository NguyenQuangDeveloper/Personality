using System.Windows;
using VSLibrary.UIComponent.MessageBox;

namespace VSP_88D_CS.Common.Helpers;

class MessageUtils
{
    public static MessageBoxResult ShowMessage(string message, 
        string title = "Vision Semicon", 
        MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None,
        MessageBoxResult autoClick = MessageBoxResult.None, int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return VsMessageBox.Show(message, title, buttons, icon, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }

    public static MessageBoxResult ShowInfo(string message, 
        string title = "Vision Semicon", 
        MessageBoxResult autoClick = MessageBoxResult.None, int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return ShowMessage(message, title, MessageBoxButton.OK, MessageBoxImage.Information, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }

    public static MessageBoxResult ShowWarning(string message,
        string title = "Vision Semicon", MessageBoxResult autoClick = MessageBoxResult.None, 
        int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return ShowMessage(message, title, MessageBoxButton.OK, MessageBoxImage.Warning, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }

    public static MessageBoxResult ShowError(string message,
        string title = "Vision Semicon", MessageBoxResult autoClick = MessageBoxResult.None, 
        int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return ShowMessage(message, title, MessageBoxButton.OK, MessageBoxImage.Error, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }

    public static MessageBoxResult ShowError(Exception ex,
        string title = "Vision Semicon", MessageBoxResult autoClick = MessageBoxResult.None,
        int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return ShowMessage(ex.Message, title, MessageBoxButton.OK, MessageBoxImage.Error, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }

    public static MessageBoxResult ShowQuestion(string message,
    string title = "Vision Semicon", MessageBoxResult autoClick = MessageBoxResult.None,
    int autoClickDelaySeconds = 0, int enableDelaySeconds = 0)
    {
        return ShowMessage(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Question, autoClick, autoClickDelaySeconds, enableDelaySeconds);
    }
}
