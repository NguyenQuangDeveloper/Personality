namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// Basic navigator interface for switching views based on ViewModel instances.
/// </summary>
public interface INavigator
{
    /// <summary>
    /// Navigates to a view based on the specified ViewModel instance.
    /// </summary>
    /// <param name="viewModel">The ViewModel to be connected to the view.</param>
    void Navigate(object viewModel);

    /// <summary>
    /// Navigates to a view based on the specified ViewModel instance within a given region.
    /// </summary>
    /// <param name="viewModel">The ViewModel to be connected to the view.</param>
    /// <param name="regionName">The name of the target region for navigation.</param>
    void Navigate(object viewModel, string regionName);
}
