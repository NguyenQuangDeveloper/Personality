using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Sequence.Constants;

namespace VSP_88D_CS.Sequence.Controllers;

public class BaseCtrl
{
    private static readonly Lazy<BaseCtrl> _instance = new Lazy<BaseCtrl>(() => new BaseCtrl());

    public static BaseCtrl Instance => _instance.Value;

    private BaseCtrl()
    {
        InspectCtrl = new InspectCtrl();
        LoadBufferCtrl = new LoadBufferCtrl();
        LoadElevatorCtrl = new LoadElevatorCtrl();
        IndexPusherCtrl = new LoadIndexPusherCtrl();
        LoadingPusherCtrl = new LoadingPusherCtrl();
        PlasmaCtrl = new PlasmaCtrl();
        UnLoadBufferCtrl = new UnLoadBufferCtrl();
        UnloadElevatorCtrl = new UnloadElevatorCtrl();
        UnloadIndexPusherCtrl = new UnloadIndexPusherCtrl();
    }

    public InspectCtrl InspectCtrl { get; private set; }
    public LoadBufferCtrl LoadBufferCtrl { get; private set; }
    public LoadElevatorCtrl LoadElevatorCtrl { get; private set; }
    public LoadIndexPusherCtrl IndexPusherCtrl { get; private set; }
    public LoadingPusherCtrl LoadingPusherCtrl { get; private set; }
    public PlasmaCtrl PlasmaCtrl;
    public UnLoadBufferCtrl UnLoadBufferCtrl { get; private set; }
    public UnloadElevatorCtrl UnloadElevatorCtrl { get; private set; }
    public UnloadIndexPusherCtrl UnloadIndexPusherCtrl { get; private set; }
}

