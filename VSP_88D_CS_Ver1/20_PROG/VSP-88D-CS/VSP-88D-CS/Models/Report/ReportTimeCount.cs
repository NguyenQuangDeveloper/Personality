using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Models.Report
{
    public class ReportTimeCount
    {
        public int FailureTime {  get; set; }     
        public int MaintTime { get; set; }
        public int RunTime { get; set; }
        public int SumWaitTime { get; set; }
        public int AssistTime { get; set; } //note
        public int AssistCount { get; set; }
        public int FailureCount { get; set; }     
        public int ProductCount { get; set; }          
    }
}
