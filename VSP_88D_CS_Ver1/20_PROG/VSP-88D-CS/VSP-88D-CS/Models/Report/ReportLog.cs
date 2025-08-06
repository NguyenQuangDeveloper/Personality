using VSLibrary.Database;

namespace VSP_88D_CS.Models.Report
{
    public class ReportLog
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id {  get; set; }
        public string DateTime { get; set; }
        public string EndTime { get; set; }
        public string ErrReset { get; set; }
        public string Contents { get; set; }
        public int Product {  get; set; }
        public int Kind { get; set; }
        public int WaitTime { get; set; }
        public int Interval { get; set; }
        public int ErrNo { get; set; }
    }
}
