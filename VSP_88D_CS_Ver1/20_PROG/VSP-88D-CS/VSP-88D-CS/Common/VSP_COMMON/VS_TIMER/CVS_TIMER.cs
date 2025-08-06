using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_COMMON.VS_TIMER
{
    public class CVS_TIMER
    {
        private List<ulong> AccumulateTime = new List<ulong>();
        protected DateTime m_StartTick;
        protected bool m_bStart;
        protected bool m_bPaused;

        public CVS_TIMER()
        {
            Reset();
        }
        ~CVS_TIMER()
        {
            Reset();
        }

        public bool IsStarted()
        {
            return m_bStart;
        }
        public void Start()
        {
            m_StartTick = DateTime.Now;

            if (!m_bStart)
            {
                m_bStart = true;
                m_bPaused = false;
            }
        }
        public void Pause()
        {
            if (!m_bPaused)
            {
                m_bPaused = true;
                AccumulateTime.Add(GetInterval());
            }

            m_bStart = false;
        }
        public virtual void Reset()
        {
            m_bStart = false;
            m_bPaused = false;
            ClearPauseQueue();
        }
        public ulong GetInterval()
        {
            ulong ulRet = 0UL;

            if (m_bStart)
            {
                DateTime end = DateTime.Now;
                TimeSpan span = end - m_StartTick;
                ulRet = (ulong)span.TotalMilliseconds;
            }

            return ulRet;
        }
        public ulong GetElapsed()
        {
            ulong ulRet = GetAccumulatedTime() + GetInterval();
            return ulRet;
        }
        protected ulong GetAccumulatedTime()
        {
            return (ulong)AccumulateTime.Sum(v => Convert.ToInt64(v));
        }
        protected void ClearPauseQueue()
        {
            AccumulateTime.Clear();
        }
    }
    public class CWaitTimer : CVS_TIMER
    {
        private uint m_nWaitTime;
        private int m_nErrNo;

        public CWaitTimer()
        {
            m_nErrNo = -1;
            m_nWaitTime = 0;
            base.Reset();
        }
        ~CWaitTimer()
        {
        }

        public void SetTimer(uint val)
        {
            base.Reset();

            m_nWaitTime = val;
            Start();
        }
        public bool IsWaitAlarm()
        {
            if (m_bStart)
            {
                uint nInterval = (uint)GetInterval();
                if (nInterval >= m_nWaitTime)
                    return true;
            }

            return false;
        }
        public uint GetWaitTime() { return m_nWaitTime; }
        public int GetErrNo() { return m_nErrNo; }
    }

    public class CDelayTimer : CVS_TIMER
    {
        private uint m_nDelayTime;

        public CDelayTimer()
        {

        }
        ~CDelayTimer()
        {

        }

        public void SetTimer(uint val)
        {
            m_nDelayTime = val;
        }
        public bool IsRemainDelay()
        {
            if (m_bStart)
            {
                if (GetInterval() < m_nDelayTime)
                {
                    return true;
                }
                else
                {
                    Reset();
                    return false;
                }
            }

            return false;
        }
        public virtual void Reset()
        {
            base.Reset();

            m_nDelayTime = 0;
        }
        public uint GetRemainTime(uint nDefault)
        {
            uint nRet = nDefault;

            if (IsRemainDelay())
                nRet = m_nDelayTime - (uint)GetInterval();

            return nRet;
        }
    }
}
