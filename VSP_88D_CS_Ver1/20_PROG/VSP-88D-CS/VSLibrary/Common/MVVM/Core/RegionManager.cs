using System.Windows;
using System.Windows.Controls;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.MVVM.Core;

/// <summary>
/// Manages regions and views in a modular MVVM architecture.  
/// Responsible for navigating between views and binding the appropriate ViewModel instances.
/// </summary>
public class RegionManager : IRegionManager
{
    /// <summary>
    /// Maintains a mapping between registered region names and the view types associated with them.
    /// Used for resolving views during navigation.
    /// </summary>
    private readonly Dictionary<string, Type> _regions = new();

    /// <summary>
    /// Holds the mapping between registered region names and their corresponding ContentControl containers,
    /// used to display views dynamically during navigation.
    /// </summary>
    private readonly Dictionary<string, ContentControl> _regionControls = new();

    /// <summary>
    /// Stores cached instances of Views based on the region name and view type,
    /// to optimize navigation performance by avoiding redundant View instantiation.
    /// </summary>
    private readonly Dictionary<(string RegionName, Type ViewType), UserControl> _viewCache = new();

    /// <summary>
    /// Gets a read-only dictionary that maps registered region names to their corresponding View types.
    /// </summary>
    public IReadOnlyDictionary<string, Type> Regions => _regions;

    /// <summary>
    /// Gets a read-only dictionary that maps registered region names to their corresponding ContentControl instances.
    /// </summary>
    public IReadOnlyDictionary<string, ContentControl> RegionControls => _regionControls;

    /// <summary>
    /// Registers a View type with the specified region.
    /// </summary>
    /// <param name="regionName">The name of the region to register.</param>
    /// <param name="viewType">The View type to be associated with the region.</param>
    public void RegisterRegion(string regionName, Type viewType)
    {
        if (!_regions.ContainsKey(regionName))
        {
            _regions[regionName] = viewType;
        }
    }

    /// <summary>
    /// Registers a <see cref="ContentControl"/> for the specified region.
    /// </summary>
    /// <param name="regionName">The name of the region to register.</param>
    /// <param name="control">The <see cref="ContentControl"/> to be associated with the region.</param>
    public void RegisterRegionControl(string regionName, ContentControl control)
    {
        if (_regionControls.ContainsKey(regionName))
        {
            return;
        }

        _regionControls[regionName] = control;
    }

    /// <summary>
    /// Returns the <see cref="Type"/> of the View mapped to the specified region.
    /// </summary>
    /// <param name="regionName">The name of the region to look up.</param>
    /// <returns>The <see cref="Type"/> of the View mapped to the region; returns <c>null</c> if not found.</returns>
    public Type? GetRegionView(string regionName)
    {
        _regions.TryGetValue(regionName, out var viewType);
        return viewType;
    }

    /// <summary>
    /// Searches all registered <see cref="ContentControl"/> instances and returns a ViewModel of the specified type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to find.</typeparam>
    /// <returns>The found ViewModel instance; returns <c>null</c> if not found.</returns>
    public TViewModel? FindViewModel<TViewModel>() where TViewModel : class
    {
        foreach (var control in _regionControls.Values)
        {
            if (control.Content is UserControl view && view.DataContext is TViewModel viewModel)
            {
                return viewModel;
            }
        }

        return null;
    }

    /// <summary>
    /// Deactivates the current ViewModel if it implements <see cref="IActivatable"/>.
    /// </summary>
    /// <param name="currentContent">The current view instance set in the <see cref="ContentControl"/>.</param>
    private void DeactivateCurrentViewModel(object? currentContent)
    {
        if (currentContent is UserControl currentView && currentView.DataContext is IActivatable currentActivatable)
        {
            currentActivatable.Deactivate();
        }
    }

    /// <summary>
    /// Activates the new ViewModel if it implements <see cref="IActivatable"/>.
    /// </summary>
    /// <param name="viewInstance">The <see cref="UserControl"/> instance to activate.</param>
    private void ActivateNewViewModel(UserControl viewInstance)
    {
        if (viewInstance.DataContext is IActivatable activatable)
        {
            activatable.Activate();
        }
    }

    /// <summary>
    /// Requests navigation to a view instance of the specified type in the given region.
    /// </summary>
    /// <param name="regionName">The name of the region to navigate to.</param>
    /// <param name="viewType">The type of the view to navigate to.</param>
    /// <exception cref="InvalidOperationException">Thrown if the region or view type is not registered.</exception>
    public void RequestNavigate(string regionName, Type viewType)
    {
        if (!_regionControls.TryGetValue(regionName, out var control))
        {
            throw new InvalidOperationException($"[Error] Region '{regionName}' is not registered.");
        }

        var cacheKey = (regionName, viewType);

        var viewInstance = _viewCache.GetValueOrDefault(cacheKey)
                          ?? CreateAndCacheView(regionName, viewType);

        DeactivateCurrentViewModel(control.Content);

        ActivateNewViewModel(viewInstance);

        control.Content = viewInstance;
    }

    /// <summary>
    /// Requests navigation to a view instance of the specified type name within the given region.
    /// </summary>
    /// <param name="regionName">The name of the region to navigate to.</param>
    /// <param name="viewTypeName">The name of the view type to navigate to.</param>
    /// <param name="viewModel">Optional: The ViewModel to assign to the view.</param>
    /// <exception cref="ArgumentException">Thrown when the viewTypeName is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the View type cannot be found or the region is not registered.</exception>
    public void RequestNavigate(string regionName, string viewTypeName, object? viewModel = null)
    {
        if (string.IsNullOrWhiteSpace(viewTypeName))
        {
            throw new ArgumentException("viewTypeName cannot be null or empty.", nameof(viewTypeName));
        }

        var viewType = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetExportedTypes())
            .FirstOrDefault(t => t.Name == viewTypeName || t.FullName?.EndsWith($".{viewTypeName}") == true)
            ?? throw new InvalidOperationException($"Unable to find a matching Type for '{viewTypeName}'.");

        if (!_regionControls.TryGetValue(regionName, out var control))
        {
            throw new InvalidOperationException($"Region '{regionName}' is not registered.");
        }

        var viewInstance = _viewCache.GetValueOrDefault((regionName, viewType));
        if (viewInstance == null)
        {
            viewInstance = CreateAndCacheView(regionName, viewType, viewModel);
        }
        else if (viewInstance is FrameworkElement fe && viewModel != null)
        {
            fe.DataContext = viewModel;
        }

        DeactivateCurrentViewModel(control.Content);
        ActivateNewViewModel(viewInstance);
        control.Content = viewInstance;
    }

    /// <summary>
    /// Requests navigation to a view based on the generic type parameter.  
    /// If the region name is not provided, it is automatically extracted from the view type.
    /// </summary>
    /// <typeparam name="TView">The type of the view to navigate to.</typeparam>
    /// <param name="regionName">
    /// The name of the region to navigate to.  
    /// If null, the region name will be inferred from the view type.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a valid region name cannot be extracted from the view type.
    /// </exception>
    public void RequestNavigate<TView>(string? regionName = null) where TView : class
    {
        var viewType = typeof(TView);

        regionName ??= ExtractRegionName(viewType);

        if (string.IsNullOrWhiteSpace(regionName))
        {
            throw new InvalidOperationException($"[Error] Failed to extract a valid region name. ViewType: {viewType.Name}");
        }

        RequestNavigate(regionName, viewType);
    }

    /// <summary>
    /// Returns the ViewModel associated with the view connected to the specified region name.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to return.</typeparam>
    /// <param name="regionName">The name of the region to search for the ViewModel.</param>
    /// <returns>
    /// The ViewModel instance associated with the region's view.  
    /// Returns null if the region or ViewModel does not exist.
    /// </returns>
    public TViewModel? GetViewModel<TViewModel>(string regionName) where TViewModel : class
    {
        if (_regionControls.TryGetValue(regionName, out var control) && control.Content is UserControl view)
        {
            if (view.DataContext is TViewModel viewModel)
            {
                return viewModel;
            }
        }

        return null;
    }

    /// <summary>
    /// Creates an instance of the specified ViewType and stores it in the cache for the given region.
    /// </summary>
    /// <param name="regionName">The name of the region to associate with the view.</param>
    /// <param name="viewType">The type of the view to instantiate.</param>
    /// <returns>The created UserControl instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the specified view type cannot be instantiated or does not inherit from UserControl.
    /// </exception>
    private UserControl CreateAndCacheView(string regionName, Type viewType)
    {
        var viewInstance = Activator.CreateInstance(viewType) as UserControl
                           ?? throw new InvalidOperationException($"Cannot create an instance of ViewType '{viewType.Name}'.");

        VSContainer.Instance.ResolveView(viewInstance);
        _viewCache[(regionName, viewType)] = viewInstance;

        return viewInstance;
    }

    /// <summary>
    /// Creates an instance of the specified ViewType and stores it in the cache for the given region.
    /// Optionally assigns the specified ViewModel or resolves it dynamically via the container.
    /// </summary>
    /// <param name="regionName">The name of the region where the view will be registered.</param>
    /// <param name="viewType">The type of the view to instantiate.</param>
    /// <param name="viewModel">Optional ViewModel to assign to the view's DataContext.</param>
    /// <returns>The created UserControl instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the view type cannot be instantiated or is not a valid UserControl.
    /// </exception>
    private UserControl CreateAndCacheView(string regionName, Type viewType, object? viewModel)
    {
        var viewInstance = Activator.CreateInstance(viewType) as UserControl
                           ?? throw new InvalidOperationException($"Failed to create an instance of ViewType '{viewType.Name}'.");

        _viewCache[(regionName, viewType)] = viewInstance;

        if (viewModel != null)
        {
            viewInstance.DataContext = viewModel;
        }
        else
        {
            var viewModelType = VSContainer.Instance.GetViewModelType(viewType);

            if (viewModelType != null)
            {
                bool createNew = !_viewCache.ContainsKey((regionName, viewModelType));
                viewInstance.DataContext = VSContainer.Instance.Resolve(viewModelType, createNew, regionName);
            }
            else
            {
            }
        }

        return viewInstance;
    }

    /// <summary>
    /// Checks whether the specified region name is registered.
    /// </summary>
    /// <param name="regionName">The name of the region to check.</param>
    /// <returns>
    /// <c>true</c> if the region is registered; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsRegion(string regionName)
    {
        return _regionControls.ContainsKey(regionName);
    }

    /// <summary>
    /// Extracts the region name by replacing the 'View' suffix in the view type name with 'Region'.
    /// </summary>
    /// <param name="viewType">The view type from which to extract the region name.</param>
    /// <returns>
    /// The region name derived from the view type name.  
    /// If the view name does not end with "View", the original name is returned.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="viewType"/> is null.</exception>
    private string ExtractRegionName(Type viewType)
    {
        if (viewType == null)
        {
            throw new ArgumentNullException(nameof(viewType), "[Error] A valid viewType must be provided.");
        }

        string viewName = viewType.Name;

        return viewName.EndsWith("View", StringComparison.OrdinalIgnoreCase)
            ? viewName.Replace("View", "Region", StringComparison.OrdinalIgnoreCase)
            : viewName;
    }

    /// <summary>
    /// Automatically navigates to a View by inferring it from the given ViewModel instance.
    /// </summary>
    /// <param name="viewModel">The ViewModel instance to be displayed in the UI.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the corresponding View type cannot be found or the target region is not registered.
    /// </exception>
    public void Navigate(object viewModel)
    {
        if (viewModel == null) return;

        var viewModelType = viewModel.GetType();

        var viewTypeName = viewModelType.FullName!
            .Replace(".ViewModels.", ".Pages.")
            .Replace("ViewModel", "");

        var asm = viewModelType.Assembly;
        var viewType = asm.GetType(viewTypeName);

        if (viewType == null)
        {
            throw new InvalidOperationException($"[Navigate Error] View type '{viewTypeName}' corresponding to ViewModel '{viewModelType.Name}' could not be found.");
        }

        string regionName = ExtractRegionName(viewType);

        if (!_regionControls.TryGetValue(regionName, out var control))
        {
            throw new InvalidOperationException($"[Navigate Error] Region '{regionName}' is not registered.");
        }

        var viewInstance = _viewCache.GetValueOrDefault((regionName, viewType));
        if (viewInstance == null)
        {
            viewInstance = CreateAndCacheView(regionName, viewType, viewModel);
        }
        else
        {
            viewInstance.DataContext = viewModel;
        }

        DeactivateCurrentViewModel(control.Content);
        ActivateNewViewModel(viewInstance);

        control.Content = viewInstance;
    }

    /// <summary>
    /// Automatically navigates to a View inferred from the given ViewModel instance and applies it to the specified Region.
    /// </summary>
    /// <param name="viewModel">The ViewModel instance to be displayed.</param>
    /// <param name="regionName">The name of the Region where the View should be displayed.</param>
    /// <exception cref="ArgumentException">Thrown if the ViewModel or region name is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the View or Region cannot be found.</exception>
    public void Navigate(object viewModel, string regionName)
    {
        if (viewModel == null || string.IsNullOrWhiteSpace(regionName))
            throw new ArgumentException("[Navigate Error] ViewModel or RegionName is null or empty.");

        var viewModelType = viewModel.GetType();

        var viewTypeName = viewModelType.FullName!
            .Replace(".ViewModels.", ".Pages.")
            .Replace("ViewModel", "");

        var asm = viewModelType.Assembly;
        var viewType = asm.GetType(viewTypeName);

        if (viewType == null)
        {
            throw new InvalidOperationException($"[Navigate Error] Could not find ViewType '{viewTypeName}' corresponding to ViewModel '{viewModelType.Name}'.");
        }

        if (!_regionControls.TryGetValue(regionName, out var control))
        {
            throw new InvalidOperationException($"[Navigate Error] Region '{regionName}' is not registered.");
        }

        var viewInstance = _viewCache.GetValueOrDefault((regionName, viewType));
        if (viewInstance == null)
        {
            viewInstance = CreateAndCacheView(regionName, viewType, viewModel);
        }
        else
        {
            viewInstance.DataContext = viewModel;
        }

        DeactivateCurrentViewModel(control.Content);
        ActivateNewViewModel(viewInstance);

        control.Content = viewInstance;
    }

    public void ClearCacheView(string regionName, Type viewType)
    {
        var keysToRemove = _viewCache.Keys.Where(k => k.RegionName == regionName && k.ViewType == viewType).ToList();

        foreach (var key in keysToRemove)
        {
            _viewCache.Remove(key);
        }
    }
}
