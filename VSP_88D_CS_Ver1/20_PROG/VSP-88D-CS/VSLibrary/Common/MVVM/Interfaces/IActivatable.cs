namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// Defines methods to manage activation and deactivation lifecycle for ViewModels.
/// Implement this interface to handle activation state changes.
/// </summary>
public interface IActivatable
{
    /// <summary>
    /// Called when the ViewModel is activated.
    /// </summary>
    void Activate();

    /// <summary>
    /// Called when the ViewModel is deactivated.
    /// </summary>
    void Deactivate();
}
