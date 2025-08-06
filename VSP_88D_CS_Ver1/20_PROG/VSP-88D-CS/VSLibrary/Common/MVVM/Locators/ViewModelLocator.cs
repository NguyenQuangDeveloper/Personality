using System.Windows;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.MVVM.Locators;

/// <summary>
/// A static class that provides automatic ViewModel wiring functionality.
/// </summary>
public static class ViewModelLocator 
{
    /// <summary>
    /// The dependency injection container managing ViewModels.
    /// </summary>
    private static IContainer? _container;

    /// <summary>
    /// Initializes the ViewModelLocator with the specified container.
    /// </summary>
    /// <param name="container">The container responsible for creating and managing ViewModels.</param>
    /// <exception cref="ArgumentNullException">Thrown if the container is null.</exception>
    public static void Initialize(IContainer container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container), "Container cannot be null.");
    }

    /// <summary>
    /// Attached DependencyProperty for enabling automatic ViewModel wiring.
    /// </summary>
    public static readonly DependencyProperty AutoWireViewModelProperty =
        DependencyProperty.RegisterAttached(
            "AutoWireViewModel",
            typeof(bool),
            typeof(ViewModelLocator),
            new PropertyMetadata(false, OnAutoWireViewModelChanged));

    /// <summary>
    /// Gets the AutoWireViewModel value from the specified DependencyObject.
    /// </summary>
    /// <param name="obj">The DependencyObject to get the value from.</param>
    /// <returns>Returns whether AutoWireViewModel is enabled.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the object is null.</exception>
    public static bool GetAutoWireViewModel(DependencyObject obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "DependencyObject cannot be null.");
        return (bool)obj.GetValue(AutoWireViewModelProperty);
    }

    /// <summary>
    /// Sets the AutoWireViewModel value on the specified DependencyObject.
    /// </summary>
    /// <param name="obj">The DependencyObject to set the value on.</param>
    /// <param name="value">The value to set for AutoWireViewModel.</param>
    /// <exception cref="ArgumentNullException">Thrown if the object is null.</exception>
    public static void SetAutoWireViewModel(DependencyObject obj, bool value)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "DependencyObject cannot be null.");
        obj.SetValue(AutoWireViewModelProperty, value);
    }

    /// <summary>
    /// Callback invoked when the AutoWireViewModel property value changes.
    /// </summary>
    /// <param name="d">The DependencyObject whose property changed.</param>
    /// <param name="e">Event data for the property change.</param>
    private static void OnAutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if ((bool)e.NewValue)
        {
            AutoWireViewModel(d);
        }
    }

    /// <summary>
    /// Automatically wires the ViewModel to the specified DependencyObject.
    /// </summary>
    /// <param name="view">The DependencyObject to wire the ViewModel to.</param>
    /// <returns>The connected ViewModel object; returns null if no ViewModel is found.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the container has not been initialized.</exception>
    public static object? AutoWireViewModel(DependencyObject view)
    {
        if (_container == null)
        {
            throw new InvalidOperationException("ViewModelLocator has not been initialized. Call Initialize() first.");
        }

        if (view == null)
        {
            throw new ArgumentNullException(nameof(view), "View object cannot be null.");
        }

        var viewType = view.GetType();
        var viewModelType = _container.GetViewModelType(viewType);

        if (viewModelType == null)
        {
            return null;
        }

        var viewModel = _container.Resolve(viewModelType);

        if (view is FrameworkElement frameworkElement)
        {
            frameworkElement.DataContext = viewModel;
        }
        else
        {
        }

        return viewModel;
    }
}
