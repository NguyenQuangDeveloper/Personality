using System.Windows.Controls;

namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// Provides mapping and navigation functionalities between Regions, Views, and ViewModels.
/// </summary>
public interface IRegionManager : INavigator
{
    /// <summary>
    /// Registers a View type with the specified Region name.
    /// </summary>
    /// <param name="regionName">The name of the Region to register.</param>
    /// <param name="viewType">The View type to associate with the Region.</param>
    void RegisterRegion(string regionName, Type viewType);

    /// <summary>
    /// Requests navigation to a View of the specified type within a given Region.
    /// </summary>
    /// <param name="regionName">The name of the Region to navigate.</param>
    /// <param name="viewType">The type of the View to navigate to.</param>
    void RequestNavigate(string regionName, Type viewType);

    /// <summary>
    /// Requests navigation by Region and View type name, optionally with a ViewModel instance.
    /// </summary>
    /// <param name="regionName">The name of the Region.</param>
    /// <param name="viewTypeName">The name of the View class as string.</param>
    /// <param name="viewModel">Optional ViewModel instance.</param>
    void RequestNavigate(string regionName, string viewTypeName, object? viewModel = null);

    /// <summary>
    /// Requests navigation to a Region using a generic View type.
    /// </summary>
    /// <typeparam name="TView">The type of the View to navigate to.</typeparam>
    /// <param name="regionName">The Region name; if null, default is used.</param>
    void RequestNavigate<TView>(string? regionName = null) where TView : class;

    /// <summary>
    /// Returns the View type registered to the specified Region name.
    /// </summary>
    /// <param name="regionName">The Region name to query.</param>
    /// <returns>The registered View type or null if none exists.</returns>
    Type? GetRegionView(string regionName);

    /// <summary>
    /// Registers a ContentControl associated with the specified Region name.
    /// </summary>
    /// <param name="regionName">The Region name.</param>
    /// <param name="control">The ContentControl to map.</param>
    void RegisterRegionControl(string regionName, ContentControl control);

    /// <summary>
    /// Finds and returns a specific ViewModel currently displayed on the UI.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to find.</typeparam>
    /// <returns>The ViewModel instance or null if not found.</returns>
    TViewModel? FindViewModel<TViewModel>() where TViewModel : class;

    /// <summary>
    /// Returns the ViewModel instance registered within a specific Region.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel.</typeparam>
    /// <param name="regionName">The Region name.</param>
    /// <returns>The ViewModel instance or null if not found.</returns>
    TViewModel? GetViewModel<TViewModel>(string regionName) where TViewModel : class;

    /// <summary>
    /// Checks whether a Region with the specified name is registered.
    /// </summary>
    /// <param name="regionName">The Region name to check.</param>
    /// <returns><c>true</c> if the Region is registered; otherwise, <c>false</c>.</returns>
    bool ContainsRegion(string regionName);

    void ClearCacheView(string regionName, Type viewType);
}
