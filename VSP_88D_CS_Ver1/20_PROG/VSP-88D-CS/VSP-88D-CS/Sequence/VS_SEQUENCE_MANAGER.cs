using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Common;

namespace VSP_88D_CS.Sequence
{
    public class VS_SEQUENCE_MANAGER
    {
        private static readonly VS_SEQUENCE_MANAGER instance = new VS_SEQUENCE_MANAGER();
        public static VS_SEQUENCE_MANAGER Instance => instance;

        #region Property
        private bool m_bDoorLockProcEnable;
        public bool IsDoorLockProcEnable() => m_bDoorLockProcEnable; 
        public void SetDoorLockProcEnable(bool val) => m_bDoorLockProcEnable = val;  
        #endregion

        public VS_SEQUENCE_MANAGER() 
        { 

        }

        public bool AnyOneInitializing()
        {
            throw new NotImplementedException();
        }

        public bool IsInManualFunction()
        {
            throw new NotImplementedException();
        }
    }
}
