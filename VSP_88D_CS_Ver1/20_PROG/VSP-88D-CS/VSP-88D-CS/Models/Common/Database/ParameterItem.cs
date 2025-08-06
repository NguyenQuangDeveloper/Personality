using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Database;
using VSP_88D_CS.Models.Setting;

namespace VSP_88D_CS.Models.Common.Database
{
    public class ParameterItem : BlazorViewModelBase, IParameterItem
    {
        private string _parameter;
        private object _value;

        private int _minLevel;
        private int _maxLevel;
        private double _minValue;
        private double _maxValue;
        private ParameterType _type;
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

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [UniqueGroup("SectionKeyGroup")]
        public string Section { get; set; }

        [NotNull]
        [UniqueGroup("SectionKeyGroup")]
        public string Key { get; set; }

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
            set => SetProperty(ref _isModified, value);
        }

        public object Value
        {
            get => _value;
            set
            {
                {
                    IsModified = true; 
                    if (Type == ParameterType.ComboBox)
                    {
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
                        _value = ApplyValidation(value);
                    }

                    if (Type == ParameterType.CheckBox)
                    {
                        if (value is bool boolValue)
                        {
                            _value = boolValue ? true : false;
                        }
                        else if (value is string stringValue)
                        {
                            _value = stringValue == "1" ? true : false;
                        }
                        else if (value is int intValue)
                        {
                            _value = intValue == 1 ? true : false;
                        }
                        else
                        {
                            _value = false;
                        }
                        UpdateDisplayText();
                    }
                    OnValueChanged();

                    SetProperty(ref _value, value);
                }
            }
        }
    
        public int MinLevel
        {
            get => _minLevel;
            set => SetProperty(ref _minLevel, value);
        }
    
        public int MaxLevel
        {
            get => _maxLevel;
            set => SetProperty(ref _maxLevel, value);
        }
    
        public double MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }
  
        public double MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }
    
        public ParameterType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        
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
        
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }    

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
      
        [IgnoreColumn]
        //public LoginLevel LoginLevel
        //{
        //    get => _loginLevel;
        //    set => SetProperty(ref _loginLevel, value);
        //}
     
        public Validation Validation
        {
            get => _validation;
            set => SetProperty(ref _validation, value);
        }
      
        public object DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }      
        public string DisplayText
        {
            get => _displayText;
            set => SetProperty(ref _displayText, value);
        }


        [IgnoreColumn]
        public string AdditionalText
        {
            get => _additionalText;
            set => SetProperty(ref _additionalText, value);
        }

        public string CheckedText
        {
            get => _checkedText;
            set => SetProperty(ref _checkedText, value);
        }

        public string UncheckedText
        {
            get => _uncheckedText;
            set => SetProperty(ref _uncheckedText, value);
        }

        public string Tooltip
        {
            get => _tooltip;
            set => SetProperty(ref _tooltip, value);
        }
        [IgnoreColumn]
        public ICommand BrowseCommand { get; }

        public ParameterItem()
        {
            BrowseCommand = new RelayCommand(OnBrowse);
            Value = Value ?? false;  
        }

        public ParameterItem(ParameterItem other) 
        {
            BrowseCommand = new RelayCommand(OnBrowse);

            Section = other.Section;
            Key = other.Key;
            Value = other.Value;
            Parameter = other.Parameter;
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

        public event Action<ParameterItem> ValueChanged;

        protected virtual void OnValueChanged()
        {
            if (Type == ParameterType.ComboBox)
            {
                ValueChanged?.Invoke(this);
            }
        }

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
                else if (strValue.ToUpper().Equals("FALSE"))
                {
                    DisplayText = uncheckedText;
                    AdditionalText = checkedText;
                }
                else
                {
                    DisplayText = strValue;
                    AdditionalText = "";
                }
            }
            else
            {
                DisplayText = Value?.ToString() ?? string.Empty;
                AdditionalText = "";
            }
        }

        private object ApplyValidation(object value)
        {
            if (Validation == null || value == null)
            {
                return value;
            }

            try
            {
                if (value is string strValue && double.TryParse(strValue, out double parsedValue))
                {
                    value = parsedValue;
                }

                if (value is IComparable comparableValue)
                {
                    var targetType = value.GetType();

                    if (MinValue != null)
                    {
                        var minValue = ConvertToComparable(targetType, MinValue);
                        if (comparableValue.CompareTo(minValue) < 0)
                        {
                            return minValue;
                        }
                    }

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
            throw new InvalidOperationException("Unsupported type for comparison");
        }
    }
}
