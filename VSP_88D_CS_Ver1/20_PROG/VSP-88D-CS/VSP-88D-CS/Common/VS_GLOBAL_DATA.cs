using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Sequence.Constants;
using VSP_COMMON;
using ZstdSharp.Unsafe;
namespace VSP_88D_CS.Common
{
    public class VS_GLOBAL_DATA
    {
        private List<string> _strRcpName { get; set; }
        //private eRcpMode _nRcpMode;

        #region Property
        private bool m_bAutoRun;
        public bool GetAutoRun() => m_bAutoRun;
        public void SetAutoRun(bool bVal) => m_bAutoRun = bVal;

        private bool m_bDryRun;
        public bool GetDryRun() => m_bDryRun;
        public void SetDryRun(bool bVal) => m_bDryRun = bVal;

        private bool m_bErrFlag;
        public bool GetErrFlag() => m_bErrFlag;
        public void SetErrFlag(bool bVal) => m_bErrFlag = bVal;

        private bool m_bFlushRun;
        public bool GetFlushRun() => m_bFlushRun;
        public void SetFlushRun(bool bVal) => m_bFlushRun = bVal;

        private bool m_bIonizerOffReq;
        public bool GetIonizerOffReq() => m_bIonizerOffReq;
        public void SetIonizerOffReq(bool bVal) => m_bIonizerOffReq = bVal;

        private bool m_bEmgOn;
        public bool GetEmgOn() => m_bEmgOn; 

        private VS_RECIPE m_Recipe = new VS_RECIPE();
        public VS_RECIPE GetRecipe() => m_Recipe;


        #endregion

        public VS_GLOBAL_DATA()
        {
            InitValues();
        }

        private void InitValues()
        {
            m_bAutoRun = false;
            m_bDryRun = false;
            m_bErrFlag = false;
            m_bFlushRun = false;
        }
        public void SetEmgOn( bool bVal)
        {
            throw new NotImplementedException();
        }

        public (double pos, double vel, double acc) GetParamMotion(int nAxis,int nPosID )
        {
            double pos = 0, vel = 0, acc = 0;
            pos = m_Recipe.GetPosition( nAxis, nPosID );
            vel = m_Recipe.GetVelocity( nAxis, nPosID );
            acc = m_Recipe.GetAccel( nAxis, nPosID );
            return (pos, vel, acc);
        }
    }
}
