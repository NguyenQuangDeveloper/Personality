using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Common.Database;

namespace VSP_88D_CS.Common.CIM
{
    public enum Command
    {
        Unknown = 0,
        True = 1,
        False = 2
    }
    public enum GemStatus
    {
        LastGem = 0,
        PrevLGIT = 1
    }
    public class CimState
    {
        private static CimState? _instance = new CimState();
        private static readonly object _lock = new object();

        public int commState;
        public int controlState;
        public int prevControlState;
        public int latestControlState;

        public int processState;
        public int prevProcessState;

        public bool machineIdleOn;
        public int machineStatePls;
        public int prevMachineStatePls;

        public int dispFg;
        public int softChkCnt;
        public int time500msCnt;
        public bool connectedFg;
        public bool doorOpenFg;
        public bool establish;
        public bool enabled;

        public static CimState GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CimState();
                    }
                }
            }
            return _instance;
        }
        // Constructor
        public CimState()
        {

        }
    }

}
