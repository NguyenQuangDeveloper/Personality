using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using VSLibrary.Common.MVVM.Core;

namespace VSLibrary.Common.MVVM.Behaviors;

/// <summary>
/// A WPF behavior that automatically registers and manages a Region on a ContentControl.
/// </summary>
public class RegionBehavior : Behavior<ContentControl>
{
    /// <summary>
    /// Attached DependencyProperty for specifying the Region name
    /// associated with a ContentControl. Used to map Views dynamically.
    /// </summary>
    public static readonly DependencyProperty RegionNameProperty =
        DependencyProperty.RegisterAttached(
            "RegionName",
            typeof(string),
            typeof(RegionBehavior),
            new PropertyMetadata(OnRegionNameChanged));

    /// <summary>
    /// Retrieves the RegionName value from the specified <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The object from which to retrieve the RegionName.</param>
    /// <returns>The Region name associated with the object.</returns>
    public static string GetRegionName(DependencyObject obj) => (string)obj.GetValue(RegionNameProperty);

    /// <summary>
    /// Sets the RegionName value on the specified <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="obj">The object on which to set the RegionName.</param>
    /// <param name="value">The Region name to assign.</param>
    public static void SetRegionName(DependencyObject obj, string value) => obj.SetValue(RegionNameProperty, value);

    /// <summary>
    /// An external callback action used to resolve and register regions.
    /// </summary>
    private static Action<string, ContentControl>? _resolveRegionAction;

    /// <summary>
    /// A list of registered region names used to prevent duplicate registration.
    /// </summary>
    private static readonly HashSet<string> _registeredRegions = new();

    /// <summary>
    /// Sets the action used to resolve regions dynamically.
    /// </summary>
    /// <param name="resolveRegionAction">
    /// An action that takes a region name and a <see cref="ContentControl"/> 
    /// to associate the region with.
    /// </param>
    public static void ConfigureRegionResolver(Action<string, ContentControl> resolveRegionAction)
    {
        _resolveRegionAction = resolveRegionAction;
    }

    /// <summary>
    /// Callback invoked when the <c>RegionName</c> attached property is changed.
    /// </summary>
    /// <param name="d">The <see cref="DependencyObject"/> where the property was changed.</param>
    /// <param name="e">Event arguments that contain information about the property change.</param>
    /// <remarks>
    /// This method registers the region with the <see cref="RegionManager"/> and ensures it is only registered once.
    /// It also invokes the externally provided resolver action if available.
    /// </remarks>
    /// <exception cref="Exception">Thrown when the region registration fails.</exception>
    private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ContentControl contentControl || e.NewValue is not string regionName)
        {
            return;
        }

        if (_registeredRegions.Contains(regionName))
        {
            return;
        }

        try
        {
            _registeredRegions.Add(regionName);

            if (VSContainer.Instance.RegionManager is RegionManager regionManager)
            {
                regionManager.RegisterRegion(regionName, contentControl.GetType());
                regionManager.RegisterRegionControl(regionName, contentControl);
            }

            _resolveRegionAction?.Invoke(regionName, contentControl);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to register region '{regionName}'", ex);
        }
    }

    /// <summary>
    /// Called when the behavior is attached to a <see cref="ContentControl"/>.
    /// </summary>
    /// <remarks>
    /// Registers the region name and invokes the region resolver action if provided.
    /// </remarks>
    /// <exception cref="Exception">Thrown if the region registration fails.</exception>
    protected override void OnAttached()
    {
        base.OnAttached();

        var regionName = GetRegionName(AssociatedObject);
        if (string.IsNullOrEmpty(regionName))
        {
            return;
        }

        if (_registeredRegions.Contains(regionName))
        {
            return;
        }

        try
        {
            _registeredRegions.Add(regionName);
            _resolveRegionAction?.Invoke(regionName, AssociatedObject);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to register region '{regionName}'", ex);
        }
    }
}
