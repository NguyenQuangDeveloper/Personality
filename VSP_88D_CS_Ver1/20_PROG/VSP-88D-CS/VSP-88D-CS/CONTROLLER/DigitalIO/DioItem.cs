using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Controller;

namespace VSP_88D_CS.CONTROLLER.DigitalIO
{
    public class DioItem : IIOSettinglist
    {
        public IOType IOType { get; set; }
        public string WireName { get; set; } = string.Empty;
        public string EmName { get; set; } = string.Empty;
        public string StrdataName { get; set; } = string.Empty;
    }
}
