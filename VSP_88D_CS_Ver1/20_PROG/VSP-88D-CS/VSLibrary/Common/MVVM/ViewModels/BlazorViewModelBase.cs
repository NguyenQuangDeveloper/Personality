using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VSLibrary.Common.MVVM.ViewModels;

/// <summary>
/// Base class for ViewModels used in Blazor applications.
/// Provides state change notifications and property change management.
/// </summary>
public class BlazorViewModelBase : ViewModelBase
{
    /// <summary>
    /// Event for notifying state changes in Blazor.
    /// </summary>
    public event Action? StateHasChanged;

    /// <summary>
    /// Method to notify that the state has changed.
    /// </summary>
    /// <param name="stateChangeAction">Optional additional action to execute on state change.</param>
    protected void NotifyStateChanged(Action? stateChangeAction = null)
    {
        stateChangeAction?.Invoke();
        StateHasChanged?.Invoke();
    }

    /// <summary>
    /// Sets a property value and updates the state on property change.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">The backing field of the property.</param>
    /// <param name="value">The new value to set.</param>
    /// <param name="propertyName">The name of the property. Automatically set.</param>
    /// <returns>True if the value was changed; otherwise, false.</returns>
    protected new bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false; // 값이 변경되지 않음
        }

        field = value;
        OnPropertyChanged(propertyName);
        NotifyStateChanged();
        return true; // 값이 변경됨
    }

    /// <summary>
    /// Notifies that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the changed property.</param>
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Initialization method to be called from Blazor components.
    /// </summary>
    public async Task InitializeAsync()
    {
        await OnInitializeAsync();
        NotifyStateChanged();
    }

    /// <summary>
    /// Virtual initialization method for overriding in derived ViewModels.
    /// </summary>
    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// The INotifyPropertyChanged event.
    /// </summary>
    public new event PropertyChangedEventHandler? PropertyChanged;
}
