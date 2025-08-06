using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace VSLibrary.Common.MVVM.Locators;

/// <summary>
/// Converter class that automatically connects a ViewModel to a View using the ViewModelLocator.
/// </summary>
public class ViewModelLocatorConverter : IValueConverter
{
    /// <summary>
    /// Automatically wires a ViewModel to the specified View.
    /// </summary>
    /// <param name="value">The View to connect the ViewModel to. Must be of type <see cref="DependencyObject"/>.</param>
    /// <param name="targetType">The target binding type. (Not used)</param>
    /// <param name="parameter">Additional parameter. (Not used)</param>
    /// <param name="culture">Culture information. (Not used)</param>
    /// <returns>The connected ViewModel object, or null if no ViewModel is found.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a DependencyObject.</exception>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DependencyObject view)
        {
            throw new ArgumentException($"'{nameof(value)}' must be a DependencyObject.", nameof(value));
        }

        var viewModel = ViewModelLocator.AutoWireViewModel(view);

        if (viewModel == null)
        {
            return null;
        }

        return viewModel;
    }

    /// <summary>
    /// ConvertBack is not supported by <see cref="ViewModelLocatorConverter"/>.
    /// </summary>
    /// <param name="value">The value to convert back. (Not used)</param>
    /// <param name="targetType">The target type. (Not used)</param>
    /// <param name="parameter">Additional parameter. (Not used)</param>
    /// <param name="culture">Culture information. (Not used)</param>
    /// <returns>Always throws <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown when ConvertBack is called.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var errorMessage = $"{nameof(ViewModelLocatorConverter)} does not support ConvertBack.";
        throw new NotSupportedException(errorMessage);
    }
}
