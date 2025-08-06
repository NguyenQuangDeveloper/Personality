using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_COMMON.RECIPE_PARAM;

namespace VSP_COMMON
{
    public class VS_RECIPE
    {
        private bool[] _loaded = new bool[(int)eRcpType.RECIPE_TYPE_MAX];
        public string strLogHead;

        private TMotionParam _motionVal = new TMotionParam();
        private TCleanParam _cleanVal = new TCleanParam();
        private TLaneOtherParam _laneOtherVal = new TLaneOtherParam();

        public void Clear()
        {
            _motionVal = new TMotionParam();
            _cleanVal = new TCleanParam();
            _laneOtherVal = new TLaneOtherParam();

            for (int i = 0; i < _loaded.Length; i++)
                _loaded[i] = false;

            strLogHead = string.Empty;
        }
        public TMotionParam GetMotionRcp() => _motionVal;
        public void SetMotionRcp(TMotionParam mtrVal) => _motionVal = mtrVal;
        public TCleanParam GetCleanRcp() => _cleanVal;
        public void SetCleanRcp(TCleanParam clnVal) => _cleanVal = clnVal;

        public TLaneOtherParam GetLaneOtherRcp() => _laneOtherVal;
        public void SetLaneOtherRcp(TLaneOtherParam laneOTherVal) => _laneOtherVal = laneOTherVal;

        public double GetMaxLimit(int motor) => _motionVal.GetMaxLimit(motor);
        public double GetMinLimit(int motor) => _motionVal.GetMinLimit(motor);
        public double GetPosition(int motor, int posId) => _motionVal.GetPosition(motor, posId);
        public double GetVelocity(int motor, int posId) => _motionVal.GetVelocity(motor, posId);
        public double GetAccel(int motor, int posId) => _motionVal.GetAccel(motor, posId);

        public double GetStartVac(int step) => _cleanVal.CleanItems[step].dStartVac;
        public int GetGasSp(int step, int gas) => _cleanVal.CleanItems[step].GetGasSp(gas);
        public int GetRfPower(int step) => _cleanVal.CleanItems[step].nRfPower;
        public int GetStepTimeSet(int step) => _cleanVal.CleanItems[step].nStepTime;
        public double GetOverPressVal() => _cleanVal.dOverPress;
        public int GetOverPressTime() => _cleanVal.nOverPressTime;
        public int GetTotalCleanTime() => _cleanVal.GetTotalTime();

        public bool IsLoadedAll() => _loaded.All(l => l);
        public bool IsLoaded(int type) => type >= 0 && type < _loaded.Length && _loaded[type];

        public void ResetCleanParam() => _cleanVal = new TCleanParam();
        
        public int GetMgzSlotCnt() =>_laneOtherVal.GetMgzSlotCnt();
    }

    public class RecipeExtern
    {
        public static bool IsCorrectStep(TCleanItem item)
        {
            return true;
        }

    }
}
