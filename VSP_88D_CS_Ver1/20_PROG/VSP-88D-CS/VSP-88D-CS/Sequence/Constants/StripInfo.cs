using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_88D_CS.Sequence.Constants;



public class StripInfo
{
    public List<StripID> Strips { get; set; }

    public StripInfo()
    {
        Strips = new List<StripID>(5); // Max Lane is 5
    }
}

public class StripID
{
    public eStripState StripState { get; set; }
    public eStripLocation Location { get; set; }
    public short Lane { get; set; }
}

public enum eStripState
{
    UNKNOW,
    EXIST,
    EMPTY,
    READ_OK,
    READ_NG,
    STRIP_CHECK_OK,
    STRIP_CHECK_NG,
    PLASMAED,
    DONE
}

public enum eStripLocation
{
    UNKNOW,
    LOADED,
    IN_CHAMBER,
    UNLOAD,
    END
}
