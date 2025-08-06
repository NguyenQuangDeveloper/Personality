using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.MVVM.ViewModels;

/// <summary>
/// Base class for ViewModels.
/// All ViewModels following the MVVM pattern can inherit from this class.
/// </summary>
public class ViewModelBase : ObservableObject, IActivatable
{
    /// <summary>
    /// Sets a property value and raises the property changed notification.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="storage">The current stored value.</param>
    /// <param name="value">The new value to set.</param>
    /// <param name="propertyName">The name of the property (automatically set).</param>
    /// <returns>True if the value changed; otherwise, false.</returns>
    protected new bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null!)
    {
        return base.SetProperty(ref storage, value, propertyName);
    }

    /// <summary>
    /// Automatically sets all string properties of the ViewModel based on database data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="db">A list containing multilingual data.</param>
    /// <param name="language">The language to set (e.g., "Kor", "Eng", "Use1", "Use2").</param>
    /// <exception cref="ArgumentException">Thrown when the data list is null or empty.</exception>
    protected void SetLanguage<T>(IEnumerable<T> db, string language) where T : class
    {
        if (db == null || !db.Any())
        {
            throw new ArgumentException("The database list is null or empty.", nameof(db));
        }

        foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.CanRead && property.CanWrite)
            {
                SetLocalizedProperty(db, language, property);
            }
        }

        foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            SetLocalizedField(db, language, field);
        }
    }

    /// <summary>
    /// Sets a specific property using multilingual data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="db">The list of multilingual data.</param>
    /// <param name="language">The language to set.</param>
    /// <param name="property">The property to set.</param>
    private void SetLocalizedProperty<T>(IEnumerable<T> db, string language, PropertyInfo property) where T : class
    {
        if (property.PropertyType == typeof(string))
        {
            var currentValue = property.GetValue(this) as string;
            if (string.IsNullOrEmpty(currentValue)) return;

            var matchingItem = FindMatchingItem(db, currentValue);
            if (matchingItem == null) return;

            var targetProperty = matchingItem.GetType().GetProperty(language);
            if (targetProperty?.PropertyType != typeof(string)) return;

            var newValue = targetProperty.GetValue(matchingItem) as string;
            if (!string.IsNullOrEmpty(newValue))
            {
                property.SetValue(this, newValue);
                OnPropertyChanged(property.Name);
            }
        }
        else if (property.PropertyType == typeof(ObservableCollection<Dictionary<string, string>>))
        {
            var collection = property.GetValue(this) as ObservableCollection<Dictionary<string, string>>;
            if (collection == null) return;

            var localizedItems = new ObservableCollection<string>(
                collection.Where(item => item.ContainsKey(language)).Select(item => item[language])
            );

            property.SetValue(this, localizedItems);
            OnPropertyChanged(property.Name);
        }
    }

    /// <summary>
    /// Sets a specific field using multilingual data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="db">The list of multilingual data.</param>
    /// <param name="language">The language to set.</param>
    /// <param name="field">The field to set.</param>
    private void SetLocalizedField<T>(IEnumerable<T> db, string language, FieldInfo field) where T : class
    {
        if (field.FieldType != typeof(string)) return;

        var currentValue = field.GetValue(this) as string;
        if (string.IsNullOrEmpty(currentValue)) return;

        var matchingItem = FindMatchingItem(db, currentValue);
        if (matchingItem == null) return;

        var targetField = matchingItem.GetType().GetProperty(language);
        if (targetField?.PropertyType != typeof(string)) return;

        var newValue = targetField.GetValue(matchingItem) as string;
        if (!string.IsNullOrEmpty(newValue))
        {
            field.SetValue(this, newValue);
            OnPropertyChanged(field.Name);
        }
    }

    /// <summary>
    /// Finds an item in the given database matching the specified value.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="db">The data list.</param>
    /// <param name="currentValue">The current value to match.</param>
    /// <returns>The matching data item, or null if none found.</returns>
    private T? FindMatchingItem<T>(IEnumerable<T> db, string currentValue) where T : class
    {
        return db.FirstOrDefault(item =>
            item.GetType().GetProperties().Any(property =>
                property.PropertyType == typeof(string) &&
                string.Equals(property.GetValue(item) as string, currentValue, StringComparison.OrdinalIgnoreCase)
            )
        );
    }

    /// <summary>
    /// Activates the ViewModel. Override for custom activation logic.
    /// </summary>
    public virtual void Activate() { }

    /// <summary>
    /// Deactivates the ViewModel. Override for custom deactivation logic.
    /// </summary>
    public virtual void Deactivate() { }
}
