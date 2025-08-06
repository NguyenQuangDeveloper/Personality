using DocumentFormat.OpenXml.Bibliography;
using EZGemPlusCS;
using OpenTK.Platform.Windows;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using VSP_88D_CS.Common.CIM;
using VSP_88D_CS.Sequence;

namespace VSP_88D_CS.Common.CIM.GEM
{
    public class GemMsg
    {
        private readonly VS_CIM_MANAGER _vsCIMManager;
        private readonly CimState _cimState;
        private readonly GlobalSystemOption _globalSystemOption;
        //private readonly VS_GLOBAL_DATA _globalData;
        private readonly object csGemLock = new object();
        private List<string> _rcmdOnly;
        private List<string> _rcmdHasCp;

        //TEST
        private short _device = 1;
        private string _ip = "127.0.0.1";
        private short _port = 5010;
        private short _passiveMode = 0; // 0: Active, 1: Passive
        private string _modelName = "VSP-88D-CS";
        private string _softRev = "V1.0.0";
        private short _t3 = 30; // Default T3 value, can be adjusted as needed
        private short _t5 = 30; // Default T5 value, can be adjusted as needed
        private short _t6 = 30; // Default T6 value, can be adjusted as needed
        private short _t7 = 30; // Default T7 value, can be adjusted as needed
        private short _t8 = 30; // Default T8 value, can be adjusted as needed
        private short _linkTest = 30;// Default T8 value, can be adjusted as needed
        private short _retry = 3; // Default Retry Count, can be adjusted as needed

        #region FOR INSTANCE
        private readonly CEZGemPlusLib _gem;
        private static GemMsg? _instance;
        private static readonly object _lock = new object();
        public static GemMsg GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new GemMsg();
                    }
                }
            }
            return _instance;
        }
        #endregion FOR INSTANCE
        public GemMsg()
        {
            _rcmdOnly = new List<string>();
            _rcmdHasCp = new List<string>();
            _vsCIMManager = VS_CIM_MANAGER.GetInstance();
            _cimState = CimState.GetInstance();
            _gem = new CEZGemPlusLib();
            _globalSystemOption = new GlobalSystemOption(); 
            //_globalData = VS_GLOBAL_DATA.Instance;
        }
        /* ==========================================================================
        Description	: Comm. Components Start/Stop
        ========================================================================== */
        public bool StartSocket()
        {
	        bool bRet = StartGem();

	        return bRet;
        }
        public bool StopSocket()
        {
	        try
            {
                int nRet = _gem.Stop();
                //_gem.DisableCommunication();
                if(nRet< 0)
                {
                    //ShowMsg(GetErrorCode(nRet) );
                    return false;
                }

                _cimState.commState = (int)COMM_STATE.COMM_DISABLED;
                SetCommStateChangeValue();

                return true;
            }
            catch (Exception e)
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"StopSocket Exception : [%s]", e->Message.c_str());
                return false;
            }
        }
        /* ==========================================================================
        Description	: Gem Initialize
        ========================================================================== */
        bool StartGem()
        {
            SetConnectionConfig();

            _rcmdOnly.Add("STOP");
            _rcmdOnly.Add("REMOTE");
            _rcmdOnly.Add("LOCAL");
            _rcmdOnly.Add("CANCEL");
            _rcmdOnly.Add("PAUSE");
            _rcmdOnly.Add("RESUME");

            _rcmdHasCp.Add("PP-SELECT");
            _rcmdHasCp.Add("START");

            for (int i = 0; i < _rcmdOnly.Count; i++)
            {
                _gem.AddRemoteCommand(_rcmdOnly[i]);
            }
            for (int i = 0; i < _rcmdHasCp.Count; i++)
            {
                _gem.AddRemoteCommand(_rcmdHasCp[i]);
            }

            ///////CEID////////////////
            AddCEIDs();
            //////SVID////////
            AddSVIDs();
            //////ALARM//////
            AddALIDs();
            /////////////////
            AddECIDs();

            // Recipe Management
            DisableAutoReply();
            SetFormatFile();
            SetFormatCode();

            _cimState.commState = (int)COMM_STATE.COMM_ENABLED_NOT_COMMUNICATING;
            SetCommStateChangeValue();

            int err = _gem.Start();

            if (err < 0)
            {
                //ShowMsg(GetErrorCode(nRet));
                return false;
            }

            err = _gem.GetRuntimeState();

            if (err < 0)
            {
                //ShowMsg(L"There is no SoftKey.");
            }

            //LOG_STR(m_strLogHead, L"GEM is Online Requested.");

            //if (Ecid->nDefaultCtrlState == DEFAULT_ONLINE_LOCAL)
            //    GemToOnlineLocal();
            //else if (Ecid->nDefaultCtrlState == DEFAULT_ONLINE_REMOTE)
            //    GemToOnlineRemote();
            //else if (Ecid->nDefaultCtrlState == DFFAULT_EQUIPMENT_OFFLINE)
            //    GemToOffline();

            return true;
        }
        void SetConnectionConfig()
        {
            _gem.DeviceID = 0;
            _gem.DeviceID = _device;
            _gem.SetIP(_ip);
            _gem.Port = _port;
            _gem.PassiveMode = _passiveMode;
            _gem.SetModelName(_modelName);
            _gem.SetSoftRev(_softRev);
            _gem.T3 = _t3; // Default T3 value, can be adjusted as needed
            _gem.T5 = _t5; // Default T5 value, can be adjusted as needed
            _gem.T6 = _t6; // Default T6 value, can be adjusted as needed
            _gem.T7 = _t7; // Default T7 value, can be adjusted as needed
            _gem.T8 = _t8; // Default T8 value, can be adjusted as needed
            _gem.LinkTestInterval = _linkTest; // Default Link Test Interval, can be adjusted as needed
            _gem.RetryCount = _retry; // Default Retry Count, can be adjusted as needed
        }

        void SetFormatFile()
        {
            string exeDir = AppContext.BaseDirectory;
            string formatFile = Path.Combine(exeDir, @"DATA\FORMAT.SML");

            _gem.SetFormatFile(formatFile);
            _gem.SetFormatCheck(true);

            string logFile = Path.Combine(_globalSystemOption.LogPath, "VS_GEM_LOG.LOG");
            _gem.SetLogFile(logFile);
        }
        void AddCEIDs()
        {
	        try
            {
                string fileName = _vsCIMManager.GetUsePreviousVIDForSTM()
                                    ? @"Data\VSP_STM_CEID.txt"
                                    : @"Data\VSP_CEID.txt";

                string exePath = AppContext.BaseDirectory;
                string sFile = Path.Combine(exePath, fileName);

                if (!File.Exists(sFile))
                {
                    //ShowMsg("CEID Definition file does not exist.");
                    return;
                }

                foreach (var line in File.ReadLines(sFile))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2)
                        continue;

                    if (!int.TryParse(parts[0].Trim(), out int ceid))
                        continue;

                    string ceidName = parts[1].Trim();

                    _gem.AddCEID(ceid, ceidName, "");
                }
            }
            catch (Exception e)
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"AddCEIDs Exception : [%s]", e->Message.c_str());
            }
        }
        void AddSVIDs()
        {
            try
            {
                string fileName = _vsCIMManager.GetUsePreviousVIDForSTM()
                    ? @"Data\VSP_STM_SVID.txt"
                    : @"Data\VSP_SVID.txt";

                string exePath = AppContext.BaseDirectory;
                string sFile = Path.Combine(exePath, fileName);

                if (!File.Exists(sFile))
                {
                    //ShowMsg("SVID Definition file does not exist.");
                    return;
                }

                var lines = File.ReadAllLines(sFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 3)
                        continue;

                    if (!int.TryParse(parts[0], out int svid))
                        continue;

                    string name = parts[1];
                    string unit = parts[2];

                    _gem.AddSVID(svid, name, unit, "");
                }
            }
            catch (Exception e)
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"AddSVIDs Exception : [%s]", e->Message.c_str());
            }
        }
        void AddALIDs()
        {
            try
            {
                string exePath = AppContext.BaseDirectory;
                string sFile = Path.Combine(exePath, @"Data\VSP_ALID.TXT");

                if (!File.Exists(sFile))
                {
                    //ShowMsg("ALID Definition file does not exist.");
                    return;
                }

                var lines = File.ReadAllLines(sFile);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(new[] { ',' }, StringSplitOptions.None)
                                    .Select(p => p.Trim()).ToList();

                    if (parts.Count < 3)
                        continue;

                    if (!int.TryParse(parts[0], out int alid))
                        continue;

                    string alarmName = parts[1];
                    string fullDescription = string.Join(" ", parts.Skip(1));

                    _gem.AddALID(alid, alarmName, fullDescription);
                }
            }
            catch (Exception e) 
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"AddALIDs Exception : [%s]", e->Message.c_str());
            }
        }
        void AddECIDs()
        {

        }
        /* ==========================================================================
        Description	: Change Control State
        ========================================================================== */
        public void GemToOnlineLocal()
        {
            try
            { 
                DisableAutoReply();
                _gem.GoOnlineLocal();
            }
            catch (Exception e)  
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME",L"GemToOnlineLocal Exception : [%s]", e->Message.c_str());
            }
        }
        //---------------------------------------------------------------------------
        public void GemToOnlineRemote()
        {
            try
            {
                DisableAutoReply();
                _gem.GoOnlineRemote();
            }
            catch (Exception e)  
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"GemToOnlineLocal Exception : [%s]", e->Message.c_str());
            }
        }
        //---------------------------------------------------------------------------
        public void GemToOffline()
        {
            EnableAutoReply();
            SetControlStateChangeValue();
            SendGemEventReport((long)CEID.CEID_OFFLINE);
            EzGemOfflineRequest(0);
        }
        void EnableAutoReply()
        {
	        try
            {
                if((int) CIM_RCMD.CIM_RCMD_S2F41 == _vsCIMManager.GetRmtCmdType() )
                    _gem.EnableAutoReply(2, 41); // 수행되면 정의된 OnS2F41
                else if((int) CIM_RCMD.CIM_RCMD_DEFAULT == _vsCIMManager.GetRmtCmdType())
                    _gem.EnableAutoReply(2, 41);
                else if((int)CIM_RCMD.CIM_RCMD_S2F49 == _vsCIMManager.GetRmtCmdType())
                    _gem.EnableAutoReply(2, 49);

                _gem.EnableAutoReply(7, 1);
                _gem.EnableAutoReply(7, 3);
                _gem.EnableAutoReply(7, 5);
                _gem.EnableAutoReply(7, 17);
                _gem.EnableAutoReply(7, 19);
                _gem.EnableAutoReply(7, 23);
                _gem.EnableAutoReply(7, 25);
            }
            catch (Exception e) 
	        {
    	        //LOG_PRINTF(L"TVS_GEM_FRAME", L"EnableAutoReply Exception : [%s]", e->Message.c_str());
            }
        }
        void DisableAutoReply()
        {
            try
            {
                if ((int)CIM_RCMD.CIM_RCMD_S2F41 == _vsCIMManager.GetRmtCmdType())
                    _gem.DisableAutoReply(2, 41); 
                else if ((int)CIM_RCMD.CIM_RCMD_DEFAULT == _vsCIMManager.GetRmtCmdType())
                    _gem.DisableAutoReply(2, 41);
                else if ((int)CIM_RCMD.CIM_RCMD_S2F49 == _vsCIMManager.GetRmtCmdType())
                    _gem.DisableAutoReply(2, 49);

                _gem.DisableAutoReply(7, 1);	// S7,F1 : Process Program Load Inquire(PPI)
                _gem.DisableAutoReply(7, 3);	// S7,F3 : Process Program Send(PPS)
                _gem.DisableAutoReply(7, 5);	// S7,F5 : Process Program Request(PPR)
                _gem.DisableAutoReply(7, 17);	// S7,F17 : Delete Process Program Send(DPS)
                _gem.DisableAutoReply(7, 19);	// S7,F19 : Current EPPD Request(RER)
                _gem.DisableAutoReply(7, 23);	// S7,F23
                _gem.DisableAutoReply(7, 25);	// S7F,25
            }
            catch (Exception e)
            {
                //LOG_PRINTF(L"TVS_GEM_FRAME", L"DisableAutoReply Exception : [%s]", e->Message.c_str());
            }
        }
        void SetControlStateChangeValue()
        {
	        SetSVID((int)SVID.SVID_CONTROLSTATE, _cimState.controlState.ToString());

	        SetSVID((int)SVID.SVID_PREV_CONTROLSTATE, _cimState.prevControlState.ToString());
        }
        void SetCommStateChangeValue()
        {
	        SetSVID((int)SVID.SVID_COMMSTATE, _cimState.controlState.ToString());
        }
        public void SetSVID(int svid, string value)
        {
            _gem.SetSVIDValue(svid, value);
        }
        /* ==========================================================================
        Description	: Send Event
        ========================================================================== */
        short SendGemEventReport(long CEID)
        {
            short nAck = 0;
            lock (csGemLock)
            {
                //long CeidVal = CimIdManager->GetCeid(CEID);

                //if (CeidVal > 0)
                //{
                //    try
                //    {
                //        if (CimManager->GetUseStripMap() && CEID_UNLDER_CARRIER_UNLD == CEID)
                //        {
                //            TCarrierInfo Carrier = JobManager->GetUnldCarrier();
                //            MakeCarrierDeviceInfoMap(Carrier);
                //        }
                //        else if (CimManager->GetUseStripMap() && CEID_JOB_END == CEID)
                //        {
                //            TVS_LOT_INFO Lot = JobManager->GetLotInfo();
                //            MakeLotMaterialInfoMap(Lot);
                //        }

                        //Send Event Report
                        //nAck = _gem.SendEventReport(CeidVal);

                //        if (nAck > 0x00)
                //        {
                //            LOG_PRINTF(m_strLogHead, L"Error GEM SendEventReport <ACK:%d - %s>", nAck, CimIdManager->GetCeidString(CEID).c_str());
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        ShowMsg(e->Message);
                //        LOG_PRINTF(m_strLogHead, L"Exception GEM SendEventReport <%s>", e->Message.c_str());
                //    }
                //    LOG_PRINTF(m_strLogHead, L"GEM SendEventReport <ID:%d %s>", CeidVal, CimIdManager->GetCeidString(CEID).c_str());
                //}
            }
            return nAck;
        }
        void EzGemOfflineRequest(long lMsgId)
        {
            try
            {
                //LOG_PRINTF(m_strLogHead, L"OfflineRequest(MsgId:%ld)", lMsgId);

                _cimState.prevControlState = _cimState.controlState;
                _cimState.controlState = (int)CONTROL_STATE.CONTROL_EQUIPMENT_OFFLINE;

                SetControlStateChangeValue();
                EnableAutoReply();
                _gem.GoOffline();

                _vsCIMManager.UpdateCommState();

                //if(_vsCIMManager.GetEcidChanged() )
                //    _vsCIMManager.SetEcidChanged(false);

                //LOG_PRINTF(m_strLogHead, L"CONNECTED\nOFFLINE\nIP:%s\nPort:%d", ((String) EzGem->IP).c_str(), EzGem->Port);
            }
            catch (Exception e) 
	        {
    	        //LOG_PRINTF(L"TVS_GEM_FRAME", L"EzGemOfflineRequest Exception : [%s]", e->Message.c_str());
            }
        }
        private void SetFormatCode()
        {
            _gem.SetFormatCode("ALID", "U4");
            _gem.SetFormatCode("SVID", "U4");
            _gem.SetFormatCode("DATAID", "U4");
            _gem.SetFormatCode("CEID", "U4");
            _gem.SetFormatCode("ECID", "U4");
            _gem.SetFormatCode("RPTID", "U4");
            _gem.SetFormatCode("TRACEID", "U4");
        }

    }
}
