using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmConfig.Models.Common;

public enum ShapeTypeDraw
{
    Ellipse_Animation,
    Rectangle_Animation,
    Ellipse_Normal,
    Rectangle_Normal,
    Text_Normal,
    Pointer,
}
public enum SelectCreateImage
{
    WithAlarmCode,
    WithAlarmName
}
public enum SaveConfigObj // Save Setting
{
    SaveAlarmSetup
}