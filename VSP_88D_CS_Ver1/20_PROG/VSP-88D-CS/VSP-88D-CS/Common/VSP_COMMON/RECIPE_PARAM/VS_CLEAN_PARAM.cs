using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using VSP_COMMON;
using System.Windows;
using System.Text.Json;
using IniParser.Model;
using IniParser;

namespace VSP_COMMON.RECIPE_PARAM
{

    using VCLEANITEM = List<TCleanItem>;

    public struct TCleanItem
    {
        public double dStartVac;
        public int nRfPower;
        public int[] nGasFlow;
        public int nStepTime;

        public TCleanItem()
        {
            dStartVac = 0.0;
            nRfPower = 0;
            nGasFlow = new int[(int)eMFCType.MAX_MFC];
            nStepTime = 0;
        }

        public void Clear()
        {
            dStartVac = 0.0;
            nRfPower = 0;
            nGasFlow = null;
            nStepTime = 0;
        }
        public void CopyFrom(TCleanItem item)
        {
            dStartVac = item.dStartVac;
            nRfPower = item.nRfPower;
            for (int i = 0; i < (int)eMFCType.MAX_MFC; i++)
            {
                nGasFlow[i] = item.nGasFlow[i];
            }
            nGasFlow = new int[(int)eMFCType.MAX_MFC];
            nStepTime = item.nStepTime;
        }

        public int GetGasSp(int gas)
        {
            if (nGasFlow != null && gas >= 0 && gas < nGasFlow.Length)
                return nGasFlow[gas];
            return 0;
        }

    }



    public class TCleanParam
    {
        public string strLogHead;
        public double dOverPress;
        public int nOverPressTime;
        public VCLEANITEM CleanItems;

        public TCleanParam()
        {
            strLogHead = "CLEAN PARAM";
            dOverPress = 0.0;
            nOverPressTime = 0;
            CleanItems = new VCLEANITEM();
        }

        public void Clear()
        {
            foreach (var item in CleanItems)
            {
                item.Clear();
            }
        }

        public void CopyFrom(TCleanParam param)
        {
            if (param == null || param.CleanItems == null)
            {
                return;
            }

            dOverPress = param.dOverPress;
            nOverPressTime = param.nOverPressTime;
            CleanItems.Clear();
            foreach (var param_item in param.CleanItems)
            {
                var item = new TCleanItem();
                item.CopyFrom(param_item);
                CleanItems.Add(item);
            }
        }

        public int GetTotalTime()
        {
            int nTotalTime = 0;
            foreach (var item in CleanItems)
            {
                nTotalTime += item.nStepTime;
            }
            return nTotalTime;
        }

        public int GetStepCount()
        {
            return CleanItems.Count;
        }

        public bool Load(string strFilePath)
        {
            if (!File.Exists(strFilePath))
            {
                MessageBox.Show($"{strLogHead}: There is no file [{strFilePath}]");
                return false;
            }
            try
            {
                //using ini-parser to read .pls file
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(strFilePath);

                // Đọc [CLEAN PARAMETER]
                CleanItems.Clear();
                var cleanSection = data["CLEAN PARAMETER"];
                foreach (var key in cleanSection)
                {
                    if (key.KeyName.StartsWith("Step_"))
                    {
                        var values = key.Value.Trim(';').Split(',');
                        var item = new TCleanItem
                        {
                            dStartVac = double.Parse(values[0]),
                            nRfPower = int.Parse(values[1]),
                            nGasFlow = new int[4]
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            item.nGasFlow[i] = int.Parse(values[2 + i]);
                        }
                        item.nStepTime = int.Parse(values[6]);
                        if (RecipeExtern.IsCorrectStep(item))
                        {
                            CleanItems.Add(item);
                        }
                        else
                        {
                            MessageBox.Show($"Wrong Clean Parameter {key.KeyName}, [{strFilePath}]");
                            return false;
                        }

                    }
                }
                var errorSection = data["ERROR PARAMETER"];
                dOverPress = double.Parse(errorSection["Over Pressure"]);
                nOverPressTime = int.Parse(errorSection["Over Pressure Time"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        public void Save(string strFilePath, bool bIsRmt)
        {
            if (File.Exists(strFilePath) && !bIsRmt)
            {
                File.Delete(strFilePath);
            }
            try
            {
                var data = new IniData();

                var cleanSection = new SectionData("CLEAN PARAMETER");
                for (int i = 0; i < CleanItems.Count; i++)
                {
                    var item = CleanItems[i];
                    string val = $"{item.dStartVac},{item.nRfPower},{item.nGasFlow[0]},{item.nGasFlow[1]},{item.nGasFlow[2]},{item.nGasFlow[3]},{item.nStepTime};";
                    cleanSection.Keys.AddKey($"Step_{i:D2}", val);
                }
                data.Sections.Add(cleanSection);

                var errorSection = new SectionData("ERROR PARAMETER");
                errorSection.Keys.AddKey("Over Pressure", dOverPress.ToString());
                errorSection.Keys.AddKey("Over Pressure Time", nOverPressTime.ToString());
                data.Sections.Add(errorSection);

                var parser = new FileIniDataParser();
                parser.WriteFile(strFilePath, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        public void MakeDefault()
        {
            strLogHead = "CLEAN PARAM DEFAULT";
            dOverPress = 0.0;
            nOverPressTime = 0;
            CleanItems.Clear();
        }

    }
}
