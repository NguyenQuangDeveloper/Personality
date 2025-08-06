using EZGemPlusCS;
using LiveChartsCore.Drawing;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.CIM;
using VSP_88D_CS.Common.CIM.GEM;

namespace VSP_88D_CS.Sequence
{
    public enum CimLotReserveState
    {
        LotReserveNone = 0,
        PpDownloaded,
        PpSelected,
        LotReserveOk,
        LotStartEnabled,
        LotStarted,
        LotEnded,
        LotProductCount,
        LotReserveFail,
        ReserveMax
    }
    public static class CimStrings
    {
        public static readonly string[] ReserveResult = new string[]
        {
                "LOT RESERVE NONE",
                "PP DOWNLOADED",
                "PP SELECTED",
                "LOT RESERVE OK",
                "LOT START ENABLED",
                "LOT STARTED",
                "LOT ENDED",
                "LOT PRODUCT CNT",
                "LOT RESERVE FAIL"
        };
    }
    public enum CimMode
    {
        None = 0,
        Reserve,
        Cancel,
        Max
    }
    public class VS_CIM_MANAGER
    {
        private static VS_CIM_MANAGER? _instance;
        private static readonly object _lock = new object();

        private readonly CimState _cimState;
        private GlobalSystemOption _sysOption;
        private GemMsg _gemMsg;
        

        public event Action<int> StatusChanged;
        public void UpdateCimConnection(int newStatus)
        {
            //Update to UI
            StatusChanged?.Invoke(newStatus);
        }

        GlobalSystemOption globalSystemOption = new GlobalSystemOption();

        // File info
        public string file;
        public string logHead;

        // Step control
        public int rcmdStep;
        public int rcmdStepUnld;

        // Port and error info
        public int port;
        public string lastError;

        // Properties
        public bool atteptOnline;
        public bool ecidChanged;

        // Carrier ID
        public Dictionary<string, int> recCarrIdMap = new Dictionary<string, int>();

        // Conversation timeout
        public bool ctWaitOn;
        public bool isCtOut;

        // Lot & Operator Info
        public string curLotId;
        public string curPpId;
        public string curCarrierId;
        public string operatorId;
        public int curCarrierCnt;

        // STM
        public bool prev2DStatus;

        public static VS_CIM_MANAGER GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new VS_CIM_MANAGER();
                    }
                }
            }
            return _instance;
        }
        public VS_CIM_MANAGER()
        {
            _cimState = CimState.GetInstance();
            _sysOption = globalSystemOption;
            //_gemMsg = GemMsg.GetInstance();

            _cimState.prevProcessState = (int)PROCESS_STATE.STATE_IDLE;
            _cimState.processState = (int)PROCESS_STATE.STATE_IDLE;

            _cimState.prevControlState = (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE;
            _cimState.controlState = (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE;
        }
        public bool IsOnlineRemote()
        {
	        return (_cimState.connectedFg && _cimState.controlState == (int)CONTROL_STATE.CONTROL_ONLINE_REMOTE);
        }
        public bool IsWaitForReplyCT()
        {
            bool bRet = false;

            if (IsOnlineRemote())
                bRet = ctWaitOn;

            return bRet;
        }
        void SetWaitForReplyCT(bool val)
        {

            ctWaitOn = val;
        }
        public void StartCtrlStateChange(int nNewCtrlState)
        {
	        if(nNewCtrlState == (int)CONTROL_STATE.CONTROL_ONLINE_LOCAL)
	        {
                _gemMsg.GemToOnlineLocal();
            }
	        else if(nNewCtrlState == (int)CONTROL_STATE.CONTROL_ONLINE_REMOTE)
	        {
                _gemMsg.GemToOnlineRemote();
            }

            else if (nNewCtrlState == (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE)
            {
                _gemMsg.GemToOffline();
                //LOG_STR(m_strLogHead, L"CIM is Offline.");
            }
        }
        public bool IsUsingGem()
        {
            if (_sysOption.GetCimType() == (int)CIMTYPE.CIM_GEM)
		        return true;

	        return false;
        }
        bool IsNormalCimType()
        {
	        if(_sysOption.GetCimType() != (int)CIMTYPE.CIM_NONE && _sysOption.GetCimType() != (int)CIMTYPE.CIM_GEM)
		        return true;

	        return false;
        }
        public void StartComm()
        {
            _gemMsg = GemMsg.GetInstance();
            if (!_cimState.enabled)
                _cimState.enabled = true;

            if (_cimState.controlState == (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE)
            {
                if (IsUsingGem())
                {
                    if (_gemMsg.StartSocket())
                        SetAtteptOnline(true);
                }
                else if (IsNormalCimType())
                {
                    //if (NormalCim->StartSocket())
                        //SetAtteptOnline(true);
                }
            }


            UpdateCommState();
        }
        public void StopComm()
        {
            if (IsUsingGem())
            {
                if (!_gemMsg.StopSocket())
                    return;

                //LOG_STR(m_strLogHead, L"GEM stop normally.");
            }
            else if (IsNormalCimType())
            {
                //NormalCim->StopSocket();
                //LOG_STR(m_strLogHead, L"CIM stop normally.");
            }

            UpdateCommStopped();
            _gemMsg.GemToOffline();
        }
        void SetAtteptOnline(bool val) 
        { 
            atteptOnline = val; 
        }
        public void UpdateCommState()
        {
	        String sBmpPath, sConnected, sEnabled;
	        if(_cimState.connectedFg)
	        {
                //Change image in CimSetting

		        //if(imgConn->HelpKeyword != L"CONNECT")
		        //{
			       // imgConn->HelpKeyword = L"CONNECT";
			       // imgConn->Picture->Bitmap->Assign(BmpConnList->GetBitmap(1) );
		        //}
            }

            else
            {
                //Change image in CimSetting

                //      if (imgConn->HelpKeyword != L"DISCONNECT")
                //{
                //          imgConn->HelpKeyword = L"DISCONNECT";
                //          imgConn->Picture->Bitmap->Assign(BmpConnList->GetBitmap(0));
                //      }
            }
            

            //CimBtnFrame->UpdateCimConnection();

            //PostMessage(Application->MainFormHandle, VS_CIM_MSG, CIM_CTRL_STATE, 0);
        }
        void UpdateCommStopped()
        {
	        _cimState.connectedFg = false;
	        _cimState.establish = false;

	        SetAtteptOnline(false);

            UpdateCommState();

            //LOG_STR(m_strLogHead, L"CIM stopped");
        }
        
        public int GetRmtCmdType()
        {
            //return CimOptVal->m_nRmtCmdType;
            return 1;
        }
        public bool GetUsePreviousVIDForSTM()
        {
            return false;
        }
    }
}
