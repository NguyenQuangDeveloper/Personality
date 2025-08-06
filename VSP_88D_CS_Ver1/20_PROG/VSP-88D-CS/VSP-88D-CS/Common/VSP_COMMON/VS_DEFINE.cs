using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSP_COMMON
{
    //public class VS_DEFINE
    //{
    public enum ALARM
    {
        SET,
        RESET
    }
    public enum eRcpType
    {
        CLEAN_PARAM = 0,
        MOTION_PARAM,
        RCP_PARAM,
        RECIPE_TYPE_MAX
    }

    public enum eRcpMode
    {
        RECIPE_SEL_LOCAL = 0,
        RECIPE_SEL_REMOTE,
        RECIPE_SEL_AGING,
        RECIPE_SEL_SLEEP
    }

    public enum eGemReicpe
    {
        RCP_EDITED = 1,
        RCP_ADDED = 2,
        RCP_DELETED = 3,
    }

    public enum eMFCType
    {
        MFC_1 = 0,
        MFC_2,
        MFC_3,
        MFC_4,
        MAX_MFC
    }
    public enum eProcessStep
    {
        NONE,
        STOP,
        RUN,
        INIT,
        E_STOP
    }
    //public class VS_DEFINE
    //{
    //}
    public enum eMtrAlarmType
    {
        NO_ALARM,
        NOT_HOME,
        AMP_ALARM,
        P_LMT_ON,
        N_LMT_ON,
        EMG_ON
    }
    public class ServoConstants
    {
        //VS_MOTION_PARAM
        public const int MAX_SERVO_POS = 12;
        //VS_LANE_OTHER_PARAM
        public const int MGZ_SLOT_MAX = 40;
        public const int MAX_CNT_TMR = 10;
    }
    
    public class FolderConstants()
    {
        public const string CIM     = "CIM";
        public const string DATA    = "DATA";
        public const string LOG     = "LOG";
        public const string REPORT  = "REPORT";
    }

    public class FileConstants
    {
        public const string SYSFILE      = "GLOBAL.INI";
        public const string OPTFILE      = "OPTION.INI";
        public const string RSVFILE      = "RESERVE.INI";
        public const string LEGENDFILE   = "LEGEND.INI";
        public const string TOWERFILE    = "TOWER.json";
        public const string LANEFILE     = "LANE.INI";
        public const string PASSLEVELFILE = "PASSLEVEL.INI";
        public const string VISIONBANKFILE  = "VISION_BANK.INI";
    }

    public enum ePM 
    {
        BTM,
        TOP,
        MAX
    }
    public enum eLayerType
    {
        SINGLE = 1,
        DUAL
    }

    public enum eLayerIdx
    { 
        LAYER_BTM = 0, 
        LAYER_TOP, 
        LAYER_MAX,
    };
}
