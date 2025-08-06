using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Models.Setting
{
    /// <summary>
    /// A class used to validate parameter values.
    /// </summary>
    public class Validation
    {
        /// <summary>
        /// Indicates the minimum acceptable value.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Indicates the maximum allowed value.
        /// </summary>
        public double MaxValue { get; set; }
    }
}
