using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Database;

namespace VSP_88D_CS.Models.Device;

public class MotionParametersItem : IMotionParametersItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int idx { get; set; }
    public int AxisNo { get; set; }
    public string AxisName { get; set; }
    public string Description { get; set; }
    public double Pos { get; set; }
    public double Vel { get; set; }
    public double Acc { get; set; }

}
