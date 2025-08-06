namespace VSLibrary.Database;

/// <summary>
/// A static container for database table model classes.
/// </summary>
public static partial class DBData
{
    /// <summary>Model for the CustTbl table.</summary>
    public class CustTbl
    {
        /// <summary>Gets or sets the customer number (Primary Key).</summary>
        [PrimaryKey]
        public int CustNo { get; set; }
        /// <summary>Gets or sets the customer name.</summary>
        public string Customer { get; set; } = string.Empty;
        /// <summary>Gets or sets the device name.</summary>
        public string DeviceName { get; set; } = string.Empty;
        /// <summary>Gets or sets the cure time.</summary>
        public DateTime CureTime { get; set; }
        /// <summary>Gets or sets the cure duration in minutes.</summary>
        public int CureMinute { get; set; }
    }

    /// <summary>Model for the DepartTbl table.</summary>
    public class DepartTbl
    {
        /// <summary>Gets or sets the department number.</summary>
        public int DepartNo { get; set; }
        /// <summary>Gets or sets the department name.</summary>
        public string DepartName { get; set; } = string.Empty;
        /// <summary>Gets or sets the record timestamp.</summary>
        public DateTime DateTime { get; set; }
    }

    /// <summary>Model for the LogTbl table.</summary>
    public class LogTbl
    {
        /// <summary>Gets or sets the log timestamp.</summary>
        public DateTime DateTime { get; set; }
        /// <summary>Gets or sets the end time.</summary>
        public DateTime EndTime { get; set; }
        /// <summary>Gets or sets the error reset time.</summary>
        public DateTime ErrReset { get; set; }
        /// <summary>Gets or sets the log contents.</summary>
        public string Contents { get; set; } = string.Empty;
        /// <summary>Gets or sets the product identifier.</summary>
        public int Product { get; set; }
        /// <summary>Gets or sets the kind identifier.</summary>
        public int Kind { get; set; }
        /// <summary>Gets or sets the wait time.</summary>
        public int WaitTime { get; set; }
        /// <summary>Gets or sets the interval.</summary>
        public int Interval { get; set; }
        /// <summary>Gets or sets the error number.</summary>
        public int ErrNo { get; set; }
        /// <summary>Gets or sets the shift identifier.</summary>
        public int Shift { get; set; }
    }

    /// <summary>Model for the RecipeTbl table.</summary>
    public class RecipeTbl
    {
        /// <summary>Gets or sets the recipe ID.</summary>
        public int ID { get; set; }
        /// <summary>Gets or sets the nCmNo.</summary>
        public int nCmNo { get; set; }
        /// <summary>Gets or sets the recipe name.</summary>
        public string Recipe { get; set; } = string.Empty;
        /// <summary>Gets or sets the record timestamp.</summary>
        public DateTime DateTime { get; set; }
        /// <summary>Gets or sets the type identifier.</summary>
        public int TYPE { get; set; }
        /// <summary>Gets or sets the clean information.</summary>
        public string Clean { get; set; } = string.Empty;
        /// <summary>Gets or sets the motion information.</summary>
        public string Motion { get; set; } = string.Empty;
    }

    /// <summary>Model for the UserTbl table.</summary>
    public class UserTbl
    {
        /// <summary>Gets or sets the user ID (Primary Key).</summary>
        [PrimaryKey]
        public string UserID { get; set; } = string.Empty;
        /// <summary>Gets or sets the user name.</summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>Gets or sets the user's grade or level.</summary>
        public int Grade { get; set; }
        /// <summary>Gets or sets the user's password.</summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>Gets or sets the user's department.</summary>
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>Model for the WorkingTbl table.</summary>
    public class WorkingTbl
    {
        /// <summary>Gets or sets the machine number.</summary>
        public int MachineNo { get; set; }
        /// <summary>Gets or sets the chamber number.</summary>
        public int ChamberNo { get; set; }
        /// <summary>Gets or sets the in-time.</summary>
        public DateTime InTime { get; set; }
        /// <summary>Gets or sets the out-time.</summary>
        public DateTime OutTime { get; set; }
        /// <summary>Gets or sets the user ID associated with the work.</summary>
        public string UserID { get; set; } = string.Empty;
        /// <summary>Gets or sets the cure temperature.</summary>
        public int CureTemp { get; set; }
        /// <summary>Gets or sets the cure duration in hours.</summary>
        public int CureHr { get; set; }
        /// <summary>Gets or sets the cure duration in minutes.</summary>
        public int CureMinute { get; set; }
        /// <summary>Gets or sets the pattern number.</summary>
        public int PtnNo { get; set; }
        /// <summary>Gets or sets the lot number.</summary>
        public int LotNo { get; set; }
    }
}
