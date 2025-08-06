using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Models.Common.Database;
using VSP_88D_CS.ViewModels.Setting.Sub;

namespace VSP_88D_CS.Models.Auto.Sub
{
    public class SvPvModel
    {
        public string RFSetValue { get; set; }
        public string RFCurentValue { get; set; }
        public string VacuumSetValue { get; set; }
        public string VacuumCurrentValue { get; set; }
        public string Gas1SetValue { get; set; }
        public string Gas2SetValue { get; set; }
        public string Gas3SetValue { get; set; }
        public string Gas4SetValue { get; set; }
        public string Gas1CurrentValue { get; set; }
        public string Gas2CurrentValue { get; set; }
        public string Gas3CurrentValue { get; set; }
        public string Gas4CurrentValue { get; set; }
        public string CleaningSetValue { get; set; }
    }
}
