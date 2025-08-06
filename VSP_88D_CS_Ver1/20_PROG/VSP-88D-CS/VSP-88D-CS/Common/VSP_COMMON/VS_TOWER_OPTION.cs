using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Common;
using VSP_COMMON.RECIPE_PARAM;
using VSP_COMMON;
using DocumentFormat.OpenXml.Vml;
using System.Text.Json;
using System.IO;
using static VS_UTILS;
namespace VSP_88D_CS.VSP_COMMON
{
    public enum eTwrKind
    { 
        TWR_RED = 0, 
        TWR_YEL, 
        TWR_GRN, 
        TWR_BUZ, 
        TOWERLAMP_KIND 
    };

    public enum eTwrMode
    {
        TWR_AUTORUN = 0, 
        TWR_STOP, 
        TWR_LOTEND, 
        TWR_ERR, 
        TOWERLAMP_MODE
    }

    public enum eLampType
    {
        LAMP_OFF = 0, 
        LAMP_ON, 
        LAMP_BLINK
    }

    public enum eBuzzType
    {
        BUZZ_OFF = 0, 
        BUZZ_ALARM
    }

    public class VS_TOWER_OPTION
    {
        private static readonly VS_TOWER_OPTION instance = new VS_TOWER_OPTION();
        public static VS_TOWER_OPTION Instance => instance;

        private string m_sFilePath;

        private int[,] nTowerVal = new int[(int)eTwrKind.TOWERLAMP_KIND, (int)eTwrMode.TOWERLAMP_MODE];

        public int[,] GetTowerLampConfig() => nTowerVal;
        private List<string> m_sTwrLampName = new List<string>
        {
            "RUN",
            "STOP",
            "JOB END",
            "ERROR"
        };

        public bool FbChanging;

        public VS_TOWER_OPTION()
        {
            m_sFilePath = System.IO.Path.Combine(GetProjectFolder(), GetDataDir(), FileConstants.TOWERFILE);
            Load();
        }

        public int GetTwrVal(eTwrKind twrKnid, eTwrMode twrMode)
        {
            return nTowerVal[(int)twrKnid, (int)twrMode] < 0 ? (int)eLampType.LAMP_OFF : nTowerVal[(int)twrKnid, (int)twrMode];
        }

        public void SetTwrVal(eTwrKind twrKnid, eTwrMode twrMode , int lampType)
        {
            int nSave = (lampType < 0) ? 0 : lampType;

            nTowerVal[(int)twrKnid, (int)twrMode] = nSave;
        }

        public string GetTwrLampMode(eTwrMode twrMode)
        {
            return m_sTwrLampName[(int)twrMode];
        }
        public void Load()
        {
            if (!File.Exists(m_sFilePath))
            {
                nTowerVal[(int)eTwrKind.TWR_RED, (int)eTwrMode.TWR_AUTORUN] = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_RED, (int)eTwrMode.TWR_STOP]    = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_RED, (int)eTwrMode.TWR_LOTEND]  = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_RED, (int)eTwrMode.TWR_ERR]     = (int)eLampType.LAMP_ON;

                nTowerVal[(int)eTwrKind.TWR_YEL, (int)eTwrMode.TWR_AUTORUN] = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_YEL, (int)eTwrMode.TWR_STOP]    = (int)eLampType.LAMP_ON;
                nTowerVal[(int)eTwrKind.TWR_YEL, (int)eTwrMode.TWR_LOTEND]  = (int)eLampType.LAMP_BLINK;
                nTowerVal[(int)eTwrKind.TWR_YEL, (int)eTwrMode.TWR_ERR]     = (int)eLampType.LAMP_OFF;

                nTowerVal[(int)eTwrKind.TWR_GRN, (int)eTwrMode.TWR_AUTORUN] = (int)eLampType.LAMP_ON;
                nTowerVal[(int)eTwrKind.TWR_GRN, (int)eTwrMode.TWR_STOP]    = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_GRN, (int)eTwrMode.TWR_LOTEND]  = (int)eLampType.LAMP_OFF;
                nTowerVal[(int)eTwrKind.TWR_GRN, (int)eTwrMode.TWR_ERR]     = (int)eLampType.LAMP_OFF;

                nTowerVal[(int)eTwrKind.TWR_BUZ, (int)eTwrMode.TWR_AUTORUN] = (int)eBuzzType.BUZZ_OFF;
                nTowerVal[(int)eTwrKind.TWR_BUZ, (int)eTwrMode.TWR_STOP]    = (int)eBuzzType.BUZZ_OFF;
                nTowerVal[(int)eTwrKind.TWR_BUZ, (int)eTwrMode.TWR_LOTEND]  = (int)eBuzzType.BUZZ_ALARM;
                nTowerVal[(int)eTwrKind.TWR_BUZ, (int)eTwrMode.TWR_ERR]     = (int)eBuzzType.BUZZ_OFF;

                //nTowerVal[(int)eTwrKind.TWR_BUZ_2, (int)eTwrMode.TWR_AUTORUN]   = (int)eBuzzType.BUZZ_OFF;
                //nTowerVal[(int)eTwrKind.TWR_BUZ_2, (int)eTwrMode.TWR_STOP]      = (int)eBuzzType.BUZZ_OFF;
                //nTowerVal[(int)eTwrKind.TWR_BUZ_2, (int)eTwrMode.TWR_LOTEND]    = (int)eBuzzType.BUZZ_ALARM;
                //nTowerVal[(int)eTwrKind.TWR_BUZ_2, (int)eTwrMode.TWR_ERR]       = (int)eBuzzType.BUZZ_OFF;
                //Save();
                return;
            }

            string json = File.ReadAllText(m_sFilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, int[]>>(json);

            for (int kind = 0; kind < (int)eTwrKind.TOWERLAMP_KIND; kind++)
            {
                string name = m_sTwrLampName[kind];
                if (data.TryGetValue(name, out int[] modeValues))
                {
                    for (int mode = 0; mode < Math.Min(modeValues.Length, (int)eTwrMode.TOWERLAMP_MODE); mode++)
                    {
                        nTowerVal[kind, mode] = modeValues[mode];
                    }
                }
            }
        }
        public void Save()
        {
            var data = new Dictionary<string, int[]>();

            for (int kind = 0; kind < (int)eTwrKind.TOWERLAMP_KIND; kind++)
            {
                var modeValues = new int[(int)eTwrMode.TOWERLAMP_MODE];
                for (int mode = 0; mode < (int)eTwrMode.TOWERLAMP_MODE; mode++)
                {
                    modeValues[mode] = (int)nTowerVal[kind, mode];
                }

                data[m_sTwrLampName[kind]] = modeValues;
            }

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(m_sFilePath, json);
        }



    }
}
