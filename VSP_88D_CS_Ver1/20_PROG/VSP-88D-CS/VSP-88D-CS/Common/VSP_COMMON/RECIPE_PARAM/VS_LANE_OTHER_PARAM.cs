using IniParser.Model;
using IniParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VSP_COMMON.RECIPE_PARAM
{

    public enum eTimeCntType
    {
        MGZ_SLOT_CNT = 0,
        LOAD_EMPTY_TIMEOUT,
        UNLD_DONE_TIMEOUT,
        ALIGNER_ON_TIME,
        AIR_BLOWER_REPEAT_CNT,
        BANK_NO,
        RECIPE_TC_MAX
    };

    public struct TLaneSkipInfo
    {
        private BitArray laneSkip;
        public TLaneSkipInfo(int laneCount)
        {
            laneSkip = new BitArray(laneCount);
        }

        public void Reset()
        {
            laneSkip.SetAll(false);
        }

        public static bool operator ==(TLaneSkipInfo left, TLaneSkipInfo right)
        {
            return left.laneSkip.Equals(right.laneSkip);
        }

        public static bool operator !=(TLaneSkipInfo left, TLaneSkipInfo right)
        {
            return !left.laneSkip.Equals(right.laneSkip);
        }
        public void CopyFrom(TLaneSkipInfo other)
        {
            for (int i = 0; i < laneSkip.Length; i++)
            {
                laneSkip[i] = other.laneSkip[i];
            }
        }

        public bool IsLaneSkipped(int index)
        {
            Debug.Assert(index >= 0 && index < laneSkip.Length, "Index out of range");
            return laneSkip[index];
        }

        public void SetLaneSkip(int index, bool skip)
        {
            Debug.Assert(index >= 0 && index < laneSkip.Length, "Index out of range");
            laneSkip[index] = skip;
        }

        public bool IsAllLanesSkipped()
        {
            return laneSkip.Cast<bool>().All(bit => bit);
        }

        public override bool Equals(object obj)
        {
            if (obj is TLaneSkipInfo other)
                return this == other;
            return false;
        }

        public override int GetHashCode()
        {
            return laneSkip.GetHashCode();
        }
    }

    public struct TTimerCountInfo
    {
        public int[] timerValues;

        public TTimerCountInfo(int recipeTcMax)
        {
            timerValues = new int[recipeTcMax];
        }

        public void Reset()
        {
            Array.Clear(timerValues, 0, timerValues.Length);
        }

        public static bool operator ==(TTimerCountInfo left, TTimerCountInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TTimerCountInfo left, TTimerCountInfo right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TTimerCountInfo other)
        {
            Array.Copy(other.timerValues, timerValues, timerValues.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is TTimerCountInfo other)
                return timerValues.SequenceEqual(other.timerValues);
            return false;
        }

        public override int GetHashCode()
        {
            return timerValues.Aggregate(17, (hash, val) => hash * 31 + val.GetHashCode());
        }
    }

    public struct TRecipeItems
    {
        public int[] recipeItem;

        public TRecipeItems(int itemMax)
        {
            recipeItem = new int[itemMax];
        }

        public void Reset()
        {
            Array.Clear(recipeItem, 0, recipeItem.Length);
        }

        public static bool operator ==(TRecipeItems left, TRecipeItems right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TRecipeItems left, TRecipeItems right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TRecipeItems other)
        {
            Array.Copy(other.recipeItem, recipeItem, recipeItem.Length);
        }

        public override bool Equals(object obj)
        {
            if (obj is TRecipeItems other)
                return recipeItem.SequenceEqual(other.recipeItem);
            return false;
        }

        public override int GetHashCode()
        {
            return recipeItem.Aggregate(17, (hash, item) => hash * 31 + item.GetHashCode());
        }
    }

    public struct TLaneOtherParam
    {
        public string strLogHead;
        public List<TLaneSkipInfo> LaneSkipOpt;
        public TTimerCountInfo TimerCount;
        public TRecipeItems OtherItems;

        public TLaneOtherParam(int layerMax)
        {
            strLogHead = "LANE PARAM";
            LaneSkipOpt = new List<TLaneSkipInfo>(Enumerable.Repeat(new TLaneSkipInfo(), layerMax));
            TimerCount = new TTimerCountInfo();
            OtherItems = new TRecipeItems();
        }

        public void Clear()
        {
            strLogHead = "LANE PARAM";
            LaneSkipOpt.Clear();
            TimerCount = new TTimerCountInfo();
            OtherItems = new TRecipeItems();
        }

        public static bool operator ==(TLaneOtherParam left, TLaneOtherParam right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TLaneOtherParam left, TLaneOtherParam right)
        {
            return !left.Equals(right);
        }

        public void CopyFrom(TLaneOtherParam other)
        {
            strLogHead = other.strLogHead;
            LaneSkipOpt = new List<TLaneSkipInfo>(other.LaneSkipOpt);
            TimerCount = other.TimerCount;
            OtherItems = other.OtherItems;
        }

        public override bool Equals(object obj)
        {
            if (obj is TLaneOtherParam other)
                return strLogHead == other.strLogHead &&
                       LaneSkipOpt.SequenceEqual(other.LaneSkipOpt) &&
                       TimerCount.Equals(other.TimerCount) &&
                       OtherItems.Equals(other.OtherItems);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(strLogHead, LaneSkipOpt, TimerCount, OtherItems);
        }

        public bool Load(string strFilePath)
        {
            if (!File.Exists(strFilePath))
            {
                MakeDefault();
                MessageBox.Show($"{strLogHead}: There is no file [{strFilePath}]");
                return false;
            }
            try
            {
                //TODO: Update load lane param function
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{strLogHead}: Error [{ex.Message}]");
                return false;
            }

            return true;
        }

        public void Save(string strFilePath, bool bIsRmt = false)
        {
            if (File.Exists(strFilePath) && !bIsRmt)
            {
                File.Delete(strFilePath);
            }
            try
            {
                //TODO: Update save lane param function
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void MakeDefault()
        {
            strLogHead = "LANE PARAM DEFAULT";
            TimerCount.timerValues[(int)eTimeCntType.MGZ_SLOT_CNT] = 20;
            TimerCount.timerValues[(int)eTimeCntType.LOAD_EMPTY_TIMEOUT] = 10;
            TimerCount.timerValues[(int)eTimeCntType.UNLD_DONE_TIMEOUT] = 10;
            TimerCount.timerValues[(int)eTimeCntType.ALIGNER_ON_TIME] = 3;
            TimerCount.timerValues[(int)eTimeCntType.AIR_BLOWER_REPEAT_CNT] = -1;
            TimerCount.timerValues[(int)eTimeCntType.BANK_NO] = -1;

            //OtherItems.recipeItem[(int)eTimeCntType.BANK_NO] = -1;
        }

        public int GetLayerSkipValue(int layer)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count ? LaneSkipOpt[layer].GetHashCode() : 0;
        }

        public void SetLayerSkipValue(int layer, int val)
        {
            if (layer >= 0 && layer < LaneSkipOpt.Count)
                LaneSkipOpt[layer] = new TLaneSkipInfo(val);
        }

        public bool IsLayerLaneSkip(int layer, int lane)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count && LaneSkipOpt[layer].IsLaneSkipped(lane);
        }

        public void SetLayerLaneSkip(int layer, int lane, bool skip)
        {
            if (layer >= 0 && layer < LaneSkipOpt.Count)
                LaneSkipOpt[layer].SetLaneSkip(lane, skip);
        }

        public bool IsLayerLaneSkipAll(int layer)
        {
            return layer >= 0 && layer < LaneSkipOpt.Count && LaneSkipOpt[layer].IsAllLanesSkipped();
        }

        public bool IsLaneSkipAll()
        {
            return LaneSkipOpt.All(lane => lane.IsAllLanesSkipped());
        }

        public int GetTotalSkipLaneCnt()
        {
            return LaneSkipOpt.Sum(lane => lane.GetHashCode());
        }

        public int GetMgzSlotCnt()
        {
            return TimerCount.timerValues[(int)eTimeCntType.MGZ_SLOT_CNT];
        }
    }
}
