using System.Windows;
using System.Windows.Controls;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.MVVM.Core;

/// <summary>
/// Manages the connection between ViewModels and Views, including screen navigation.  
/// If an <see cref="INavigator"/> is registered, it is used for navigation.  
/// Otherwise, the manager automatically generates a <see cref="Window"/> or <see cref="UserControl"/>  
/// based on the naming convention of the ViewModel.
/// </summary>
public class ViewManager : IViewManager
{
    /// <summary>
    /// Holds a reference to the dependency injection container  
    /// used for resolving ViewModels and services within the <see cref="ViewManager"/>.
    /// </summary>
    private readonly IContainer _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewManager"/> class  
    /// with the specified dependency injection container.
    /// </summary>
    /// <param name="container">
    /// The DI container used for resolving and releasing ViewModels and services.
    /// </param>
    public ViewManager(IContainer container)
    {
        _container = container;
    }

    /// <summary>
    /// Displays a view that corresponds to the specified ViewModel type.  
    /// The method infers the view type by replacing "ViewModels" with "Pages"  
    /// and removing "ViewModel" from the class name.  
    /// 
    /// If a matching <see cref="Window"/> is found, it will be shown with the ViewModel as its DataContext.  
    /// If a <see cref="UserControl"/> is found, it will be wrapped in a new <see cref="Window"/> and shown.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to resolve and bind.</typeparam>
    public void Show<TViewModel>() where TViewModel : class
    {
        var vm = _container.Resolve<TViewModel>();

        var viewTypeName = typeof(TViewModel).FullName!
            .Replace(".ViewModels.", ".Pages.")
            .Replace("ViewModel", "");

        var viewType = typeof(TViewModel).Assembly.GetType(viewTypeName);
        if (viewType == null) return;

        var instance = Activator.CreateInstance(viewType);
        if (instance is Window window)
        {
            window.DataContext = vm;
            window.Show();
        }
        else if (instance is UserControl uc)
        {
            uc.DataContext = vm;
            new Window
            {
                Content = uc,
                Width = 800,
                Height = 600
            }.Show();
        }
    }

    /// <summary>
    /// Displays a view associated with the specified ViewModel type in the given region.  
    /// If an <see cref="INavigator"/> is available, it delegates the navigation to it.  
    /// Otherwise, it falls back to a convention-based view resolution and opens the view in a new window.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the ViewModel to resolve and bind.</typeparam>
    /// <param name="regionName">The name of the region to navigate or display the view in.</param>
    public void Show<TViewModel>(string regionName) where TViewModel : class
    {
        var vm = _container.Resolve<TViewModel>();
        var navigator = TryGetNavigator();

        if (navigator != null)
        {
            navigator.Navigate(vm, regionName);
            return;
        }

        // Region 없으면 기존 방식대로 열기
        var viewTypeName = typeof(TViewModel).FullName!
            .Replace(".ViewModels.", ".Pages.")
            .Replace("ViewModel", "");

        var viewType = typeof(TViewModel).Assembly.GetType(viewTypeName);
        if (viewType == null) return;

        var instance = Activator.CreateInstance(viewType);
        if (instance is FrameworkElement fe)
        {
            fe.DataContext = vm;

            var window = new Window
            {
                Content = fe,
                Width = 800,
                Height = 600,
                Title = regionName
            };
            window.Show();
        }
    }

    /// <summary>
    /// Attempts to resolve an <see cref="INavigator"/> from the container.  
    /// Returns <c>null</c> if the navigator is not registered or resolution fails.
    /// </summary>
    /// <returns>
    /// The resolved <see cref="INavigator"/> instance, or <c>null</c> if not available.
    /// </returns>
    private INavigator? TryGetNavigator()
    {
        try
        {
            return _container.Resolve<INavigator>();
        }
        catch
        {
            return null;
        }
    }
}
