using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Models.Setting
{
    /// <summary>
    /// An enumeration that defines the types of parameters.
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Parameters in numeric format.
        /// </summary>
        Number,

        /// <summary>
        /// Parameters of type ComboBox.
        /// </summary>
        ComboBox,

        /// <summary>
        /// Parameters of type CheckBox.
        /// </summary>
        CheckBox,

        /// <summary>
        /// Parameters in text format.
        /// </summary>
        Text,

        /// <summary>
        /// Parameter to select the file path.
        /// </summary>
        FilePath,

        /// <summary>
        /// Parameter to select the directory path.
        /// </summary>
        DirectoryPath,

        /// <summary>
        /// Parameters in the title bar format. Can mean the title bar of the UI.
        /// </summary>
        TitleBar
    }
}
