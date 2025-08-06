using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace VSP_88D_CS.Models.Setting
{
    /// <summary>
    /// A class that selects an appropriate DataTemplate depending on the type of ParameterItem.
    /// </summary>
    public class ParameterTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Template for numeric type parameters.
        /// </summary>
        public DataTemplate NumberTemplate { get; set; }

        /// <summary>
        /// Template for combo box type parameter.
        /// </summary>
        public DataTemplate ComboBoxTemplate { get; set; }

        /// <summary>
        /// Template for checkbox type parameter.
        /// </summary>
        public DataTemplate CheckBoxTemplate { get; set; }

        /// <summary>
        /// Template for text type parameters.
        /// </summary>
        public DataTemplate TextTemplate { get; set; }

        /// <summary>
        /// Template for file path type parameters.
        /// </summary>
        public DataTemplate FilePathTemplate { get; set; }

        /// <summary>
        /// Template for directory path type parameters.
        /// </summary>
        public DataTemplate DirectoryPathTemplate { get; set; }

        /// <summary>
        /// Template for the title bar type parameter.
        /// </summary>
        public DataTemplate TitleBarTemplate { get; set; }

        /// <summary>
        /// Default template for read-only parameters.
        /// </summary>
        public DataTemplate ReadOnlyTemplate { get; set; }

        /// <summary>
        /// Selects an appropriate template based on the parameter type.
        /// </summary>
        /// <param name="item">The current parameter item.</param>
        /// <param name="container">The container object to which the template will be applied.</param>
        /// <returns>Returns a <see cref="DataTemplate"/> that matches the parameter type.</returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ParameterItem parameterItem)
            {
                switch (parameterItem.Type)
                {
                    case ParameterType.Number:
                        return NumberTemplate;
                    case ParameterType.ComboBox:
                        return ComboBoxTemplate;
                    case ParameterType.CheckBox:
                        return CheckBoxTemplate;
                    case ParameterType.Text:
                        return TextTemplate;
                    case ParameterType.FilePath:
                        return FilePathTemplate;
                    case ParameterType.DirectoryPath:
                        return DirectoryPathTemplate;
                    case ParameterType.TitleBar:
                        return TitleBarTemplate;
                    default:
                        return ReadOnlyTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
