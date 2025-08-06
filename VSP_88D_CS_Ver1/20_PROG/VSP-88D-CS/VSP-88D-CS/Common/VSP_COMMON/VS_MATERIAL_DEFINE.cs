using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_COMMON
{
    public class VS_MATERIAL_DEFINE
    {
        public enum eDEVICE_STATE
        {
            DEVICE_EMPTY = 0,
            DEVICE_BEFORE,
            DEVICE_WORKING,
            DEVICE_DONE,
            DEVICE_STATE_MAX
        }
        //public List<string> sDeviceState
        public List<string> sDeviceState = new List<string>()
        {
            "EMPTY",
            "BEFORE",
            "WORKING",
            "DONE"
        };
    }
}
