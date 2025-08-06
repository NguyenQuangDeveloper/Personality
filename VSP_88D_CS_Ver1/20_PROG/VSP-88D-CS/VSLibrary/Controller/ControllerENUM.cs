using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Controller
{
    /// <summary>
    /// Defines the direction of an I/O channel.
    /// </summary>
    public enum IOType
    {
        /// <summary>
        /// Input direction.
        /// </summary>
        InPut,

        /// <summary>
        /// Output direction.
        /// </summary>
        OUTPut
    }

    /// <summary>
    /// Specifies controller types for analog I/O, digital I/O, and motion control.
    /// </summary>
    public enum ControllerType
    {
        /// <summary>
        /// Ajin AXT analog I/O controller.
        /// </summary>
        AIO_AjinAXT,

        /// <summary>
        /// Adlink analog I/O controller.
        /// </summary>
        AIO_Adlink,

        /// <summary>
        /// Ajin AXT digital I/O controller.
        /// </summary>
        DIO_AjinAXT,

        /// <summary>
        /// Ajin AXT universal digital I/O controller.
        /// </summary>
        DIO_AjinAXTUniversalIO,

        /// <summary>
        /// Comizoa digital I/O controller.
        /// </summary>
        DIO_Comizoa,

        /// <summary>
        /// Ajin AXT motion controller.
        /// </summary>
        Motion_AjinAXT,

        /// <summary>
        /// Comizoa motion controller.
        /// </summary>
        Motion_Comizoa
    }
}
