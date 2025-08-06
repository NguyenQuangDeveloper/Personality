namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// Interface for managing creation and navigation of Views based on ViewModel instances.
/// </summary>
public interface IViewManager
{
    /// <summary>
    /// Finds and displays a View based on the given ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to display.</typeparam>
    void Show<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Displays the View corresponding to the specified ViewModel within the given Region.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel.</typeparam>
    /// <param name="regionName">The name of the Region where the View should be displayed.</param>
    void Show<TViewModel>(string regionName) where TViewModel : class;
}
