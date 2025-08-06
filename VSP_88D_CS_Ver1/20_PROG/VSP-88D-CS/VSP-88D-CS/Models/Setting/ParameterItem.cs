using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Database;


namespace VSP_88D_CS.Models.Setting
{
    /// <summary>
    /// The ParameterItem class is a class that manages parameter items used in the user interface.
    /// It sets values ​​for each parameter, applies validation, and can handle input data in various formats.
    /// </summary>
    public class ParameterItem : ViewModelBase, IParameterItem
	{
        private string _parameter;
        private ParameterType _type;
        private object _value;
        private string _unit;

        private int _minLevel;
        private int _maxLevel;
        private double _minValue;
        private double _maxValue;
        
        private ObservableCollection<string> _comboBoxItems = new ObservableCollection<string>();
        private bool _isEditable;
        private bool _isVisible;
        //private LoginLevel _loginLevel;
        private Validation _validation;
        private object _defaultValue;
        private string _displayText;
        private string _additionalText;
        private string _checkedText;
        private string _uncheckedText;
        private string _tooltip;

        //[PrimaryKey]
        //[AutoIncrement]
        //public int Id {  get; set; }

        ///// <summary>
        ///// Indicates the section name to which the parameter belongs.
        ///// </summary>
        //[NotNull]
        //[UniqueGroup("SectionKeyGroup")]
        public string Section { get; set; }

        /// <summary>
        /// Indicates the unique key of the parameter.
        /// </summary>
        //[NotNull]
        //[UniqueGroup("SectionKeyGroup")]
        public string Key { get; set; }

        /// <summary>
        /// Indicates the name of the parameter. Maps to the "name" property in JSON.
        /// </summary>       
        public string Parameter
        {
            get => _parameter;
            set => SetProperty(ref _parameter, value);
        }

        private bool _isModified;
        [IgnoreColumn]
        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified , value);           
        }

        /// <summary>
        /// Indicates the type of the parameter. Example: ComboBox, CheckBox, etc.
        /// </summary>      
        public ParameterType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        /// <summary>
        /// Represents the value of the parameter. Maps to the "value" property in JSON. 
        /// The value is handled differently depending on the parameter type.
        /// </summary>     
        public object Value
        {
            get => _value;
            set
            {
                //A problem occurs where the initial value does not change.
                //if (_value != value)
                {
                    IsModified = true; // Set IsModified to true when the value changes
                    if (Type == ParameterType.ComboBox)
                    {
                        // If the ComboBox value is of long type, convert it to int.
                        if (value is long longValue)
                        {
                            _value = (int)longValue;
                        }
                        else if (value is int intValue)
                        {
                            _value = intValue;
                        }
                        else if (int.TryParse(value?.ToString(), out int parsedIndex))
                        {
                            _value = parsedIndex;
                        }
                        else
                        {
                            _value = DefaultValue;
                        }
                    }
                    else
                    {
                        // Apply validation
                        _value = ApplyValidation(value);
                    }

                    // Update DisplayText if it is a CheckBox type
                    if (Type == ParameterType.CheckBox)
                    {
                        // Handling the 'CheckBox' type
                        if (value is bool boolValue)
                        {
                            // Store Boolean values ​​as is
                            _value = boolValue ? true : false;
                        }
                        else if (value is string stringValue)
                        {
                            // Convert string "1" or "0" to Boolean
                            _value = stringValue == "1" ? true : false;
                        }
                        else if (value is int intValue)
                        {
                            // Convert to integer value
                            _value = intValue == 1 ? true : false;
                        }
                        else
                        {
                            // Set default
                            _value = false;
                        }
                        UpdateDisplayText();
                    }
                    OnValueChanged();

                    SetProperty(ref _value, value);                  
                }
            }
        }

        public void SetValue(object value)
        {
            IsModified = true; // Set IsModified to true when the value changes
            if (Type == ParameterType.ComboBox)
            {
                // If the ComboBox value is of long type, convert it to int.
                if (value is long longValue)
                {
                    _value = (int)longValue;
                }
                else if (value is int intValue)
                {
                    _value = intValue;
                }
                else if (int.TryParse(value?.ToString(), out int parsedIndex))
                {
                    _value = parsedIndex;
                }
                else
                {
                    _value = DefaultValue;
                }
            }
            else
            {
                // Apply validation
                _value = ApplyValidation(value);
            }

            // Update DisplayText if it is a CheckBox type
            if (Type == ParameterType.CheckBox)
            {
                // Handling the 'CheckBox' type
                if (value is bool boolValue)
                {
                    // Store Boolean values ​​as is
                    _value = boolValue ? true : false;
                }
                else if (value is string stringValue)
                {
                    // Convert string "1" or "0" to Boolean
                    _value = stringValue == "1" ? true : false;
                }
                else if (value is int intValue)
                {
                    // Convert to integer value
                    _value = intValue == 1 ? true : false;
                }
                else
                {
                    // Set default
                    _value = false;
                }
                UpdateDisplayText();

            }
        }

        /// <summary>
        /// Indicates the minimum level for parameter values.
        /// </summary>      
        public int MinLevel
        {
            get => _minLevel;
            set => SetProperty(ref _minLevel, value);
        }

        /// <summary>
        /// Indicates the maximum level for the parameter value.
        /// </summary>     
        public int MaxLevel
        {
            get => _maxLevel;
            set => SetProperty(ref _maxLevel, value);
        }

        /// <summary>
        /// Represents the minimum value of the parameter values.
        /// </summary>       
        public double MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }

        /// <summary>
        /// Indicates the maximum value of the parameter value.
        /// </summary>       
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }


        /// <summary>
        /// ComboBox Represents a list of items.
        /// </summary>      
        [IgnoreColumn]
        public ObservableCollection<string> ComboBoxItems
        {
            get => _comboBoxItems;
            set
            {
                if (SetProperty(ref _comboBoxItems, value))
                {
                    ComboBoxItemsJson = JsonSerializer.Serialize(value);
                }
            }
        }

        public string ComboBoxItemsJson
        {
            get => JsonSerializer.Serialize(ComboBoxItems);
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _comboBoxItems = JsonSerializer.Deserialize<ObservableCollection<string>>(value);
                    OnPropertyChanged(nameof(ComboBoxItems));
                }
            }
        }

        /// <summary>
        /// Indicates whether the parameter is editable.
        /// </summary>        
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        /// <summary>
        /// Indicates whether the parameter is displayed in the user interface.
        /// </summary>       
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        /// <summary>
        /// Indicates the login permission level.
        /// </summary>      
        //[IgnoreColumn]
        //public LoginLevel LoginLevel
        //{
        //    get => _loginLevel;
        //    set => SetProperty(ref _loginLevel, value);
        //}

        /// <summary>
        /// Represents rules for validating parameters.
        /// </summary>      
        //[IgnoreColumn]
        //public Validation Validation
        //{
        //    get => _validation;
        //    set => SetProperty(ref _validation, value);
        //}

        /// <summary>
        /// Indicates the default value of the parameter.
        /// </summary>       
        public object DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }

        /// <summary>
        /// Displays the display text for the current parameter.
        /// </summary>       
        public string DisplayText
        {
            get => _displayText;
            set => SetProperty(ref _displayText, value);
        }

        /// <summary>
        /// Indicates text to be displayed additionally.
        /// </summary>
        [IgnoreColumn]
        public string AdditionalText
        {
            get => _additionalText;
            set => SetProperty(ref _additionalText, value);
        }

        /// <summary>
        /// Indicates the text to display when the CheckBox is selected.
        /// </summary>
        public string CheckedText
        {
            get => _checkedText;
            set => SetProperty(ref _checkedText, value);
        }

        /// <summary>
        /// Indicates the text to display when the CheckBox is not selected.
        /// </summary>
        public string UncheckedText
        {
            get => _uncheckedText;
            set => SetProperty(ref _uncheckedText, value);
        }

        /// <summary>
        /// Indicates a description or tip for a parameter.
        /// </summary>
        public string Tooltip
        {
            get => _tooltip;
            set => SetProperty(ref _tooltip, value);
        }

        public string Unit
        {
            get => _unit;
            set => SetProperty(ref _unit, value);
        }

        /// <summary>
        /// Command for selecting files or directories.
        /// </summary>
        [IgnoreColumn]
        public ICommand BrowseCommand { get; }

        public ParameterItem()
        {
            BrowseCommand = new RelayCommand(OnBrowse);
            Value = Value ?? false;  // Set default value to false if value is null
        }

        /// <summary>
        /// Constructor for the ParameterItem class.
        /// </summary>     
        public ParameterItem(ParameterItem other)
        {
            BrowseCommand = new RelayCommand(OnBrowse);

            //Section = other.Section;
            //Key = other.Key;
            Value = other.Value;
            Parameter = other.Parameter;
            Unit = other.Unit;
            MinLevel = other.MinLevel;
            MaxLevel = other.MaxLevel;
            MinValue = other.MinValue;
            MaxValue = other.MaxValue;
            Type = other.Type;
            ComboBoxItems = new ObservableCollection<string>(other.ComboBoxItems);
            IsEditable = other.IsEditable;
            IsVisible = other.IsVisible;
            DefaultValue = other.DefaultValue;
            DisplayText = other.DisplayText;
            CheckedText = other.CheckedText;
            UncheckedText = other.UncheckedText;
            Tooltip = other.Tooltip;
        }

        /// <summary>
        /// Apply language change.
        /// </summary>
        /// <param name="language">Language to apply (e.g. "Kor", "Eng")</param>
        public void ChangeLanguage(string language)
        {
            //if (_languageRepository == null) return;

            //var languages = _languageRepository.Data;
            //if (languages != null && languages.Any())
            //{
            //    // Change general properties
            //    SetLanguage(languages, language);

            //    // Handling ComboBoxItems changes
            //    if (ComboBoxItems != null && ComboBoxItems.Any())
            //    {
            //        var localizedItems = new ObservableCollection<string>();

            //        foreach (var item in ComboBoxItems)
            //        {
            //            // Match 4 attributes with OR condition
            //            var matchingLang = languages.FirstOrDefault(lang =>
            //                lang.Kor == item ||
            //                lang.Eng == item ||
            //                lang.Use1 == item ||
            //                lang.Use2 == item);

            //            // Set matched language data to current language
            //            var localizedValue = matchingLang != null
            //                ? matchingLang.GetType().GetProperty(language)?.GetValue(matchingLang)?.ToString()
            //                : item;

            //            localizedItems.Add(localizedValue);
            //        }

            //        ComboBoxItems = localizedItems;
            //    }
            //}
        }

        /// <summary>
        /// Opens a dialog that allows you to select a file or directory.
        /// </summary>
        private void OnBrowse()
        {
            //if (Type == ParameterType.FilePath)
            //{
            //    var dialog = new OpenFileDialog();
            //    bool? result = dialog.ShowDialog();

            //    if (result == true)
            //    {
            //        Value = dialog.FileName;
            //    }
            //}
            //else if (Type == ParameterType.DirectoryPath)
            //{
            //    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            //    {
            //        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //        if (result == System.Windows.Forms.DialogResult.OK)
            //        {
            //            Value = dialog.SelectedPath;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// An event that occurs when a value changes.
        /// </summary>
        public event Action<ParameterItem> ValueChanged;

        /// <summary>
        /// A method called when the value changes.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (Type == ParameterType.ComboBox)
            {
                ValueChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Updates the text to display based on the parameter value.
        /// </summary>
        public void UpdateDisplayText()
        {
            if (Value is bool boolValue)
            {
                string checkedText = string.IsNullOrEmpty(CheckedText) ? "Checked" : CheckedText;
                string uncheckedText = string.IsNullOrEmpty(UncheckedText) ? "Unchecked" : UncheckedText;

                AdditionalText = !boolValue ? checkedText : uncheckedText;
                DisplayText = boolValue ? checkedText : uncheckedText;
            }
            else if (Value is long || Value is int)
            {
                long longValue = Convert.ToInt64(Value);
                string checkedText = string.IsNullOrEmpty(CheckedText) ? "Checked" : CheckedText;
                string uncheckedText = string.IsNullOrEmpty(UncheckedText) ? "Unchecked" : UncheckedText;

                AdditionalText = longValue == 1 ? checkedText : uncheckedText;
                DisplayText = longValue == 0 ? checkedText : uncheckedText;
            }
            else if (Value is string strValue)
            {
                string checkedText = string.IsNullOrEmpty(CheckedText) ? "Checked" : CheckedText;
                string uncheckedText = string.IsNullOrEmpty(UncheckedText) ? "Unchecked" : UncheckedText;

                if (strValue == "1")
                {
                    DisplayText = checkedText;
                    AdditionalText = uncheckedText;
                }
                else if (strValue == "0")
                {
                    DisplayText = uncheckedText;
                    AdditionalText = checkedText;
                }
                else if (strValue.ToUpper().Equals("TRUE"))
                {
                    DisplayText = checkedText;
                    AdditionalText = uncheckedText;
                }
                else if(strValue.ToUpper().Equals("FALSE"))
                {
                    DisplayText = uncheckedText;
                    AdditionalText = checkedText;
                }
                else
                {
                    DisplayText = strValue;  // If it is a normal string, it is printed as is.
                    AdditionalText = "";
                }
            }
            else
            {
                DisplayText = Value?.ToString() ?? string.Empty;
                AdditionalText = "";
            }
        }

        /// <summary>
        /// Apply validation to parameter values.
        /// Restrict the range of values ​​so that they do not exceed the minimum and maximum values.
        /// </summary>
        private object ApplyValidation(object value)
        {
            if (_validation == null || value == null)
            {
                return value;
            }

            try
            {
                // If it is a string, try converting it to double.
                if (value is string strValue && double.TryParse(strValue, out double parsedValue))
                {
                    value = parsedValue;
                }

                // Verify minimum and maximum values ​​if they are comparable values
                if (value is IComparable comparableValue)
                {
                    var targetType = value.GetType();

                    // MinValue check
                    if (MinValue != null)
                    {
                        var minValue = ConvertToComparable(targetType, MinValue);
                        if (comparableValue.CompareTo(minValue) < 0)
                        {
                            return minValue;
                        }
                    }

                    // MaxValue check
                    if (MaxValue != null)
                    {
                        var maxValue = ConvertToComparable(targetType, MaxValue);
                        if (comparableValue.CompareTo(maxValue) > 0)
                        {
                            return maxValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return value;
        }

        /// <summary>
        /// This is a method to convert to a comparable value.
        /// </summary>
        /// <param name="type">Type to convert</param>
        /// <param name="value">Value to convert</param>
        /// <returns>Comparable value</returns>
        private IComparable ConvertToComparable(Type type, object value)
        {
            if (type == typeof(double))
            {
                return (double)Convert.ChangeType(value, typeof(double));
            }
            else if (type == typeof(int))
            {
                return (int)Convert.ChangeType(value, typeof(int));
            }
            else if (type == typeof(long))
            {
                return (long)Convert.ChangeType(value, typeof(long));
            }

            // Additional handling for other numeric types possible
            throw new InvalidOperationException("Unsupported type for comparison");
        }
    }
}
