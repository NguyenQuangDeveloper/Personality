using System.IO;
using System.Windows;
using VSP_88D_CS.Common.Device;

namespace VSP_COMMON.RECIPE_PARAM
{
    public class TMotionUnit
    {
        public double Position { get; set; }
        public double Velocity { get; set; }
        public double Acceleration { get; set; }
        public bool Jog { get; set; }

        public TMotionUnit() { }

        public TMotionUnit(double pos, double vel, double acc, bool jog = false)
        {
            Position = pos;
            Velocity = vel;
            Acceleration = acc;
            Jog = jog;
        }

        public void CopyFrom(TMotionUnit arg)
        {
            Position = arg.Position;
            Velocity = arg.Velocity;
            Acceleration = arg.Acceleration;
            Jog = arg.Jog;
        }
    }

    public struct TMotionItem
    {
        public double MinLimit { get; set; }
        public double MaxLimit { get; set; }

        public TMotionUnit[] MotionUnit { get; private set; }

        public TMotionItem()
        {
            MinLimit = 0;
            MaxLimit = 0;

            MotionUnit = new TMotionUnit[ServoConstants.MAX_SERVO_POS];
            for (int i = 0; i < MotionUnit.Length; i++)
            {
                MotionUnit[i] = new TMotionUnit();
            }
        }

        public void CopyFrom(in TMotionItem arg)
        {
            MinLimit = arg.MinLimit;
            MaxLimit = arg.MaxLimit;

            if (MotionUnit == null || MotionUnit.Length != ServoConstants.MAX_SERVO_POS)
                MotionUnit = new TMotionUnit[ServoConstants.MAX_SERVO_POS];

            for (int i = 0; i < MotionUnit.Length; i++)
            {
                if (MotionUnit[i] == null)
                    MotionUnit[i] = new TMotionUnit();

                MotionUnit[i].CopyFrom(arg.MotionUnit[i]);
            }
        }

        public double GetPosition(int posId) => MotionUnit[posId].Position;
        public double GetVelocity(int posId) => MotionUnit[posId].Velocity;
        public double GetAccel(int posId) => MotionUnit[posId].Acceleration;
    }

    public struct TMotionParam
    {
        public string strLogHead = "MOTION PARAM";
        public TMotionItem[] MotParam { get; private set; }
        public TMotionParam()
        {
            strLogHead = "MOTION PARAM";
            MotParam = new TMotionItem[(int)eMtr.MAX_SERVO_AXIS];

            for (int i = 0; i < MotParam.Length; i++)
            {
                MotParam[i] = new TMotionItem();
            }

        }
        public void Clear()
        {
            for (int i = 0; i < MotParam.Length; i++)
            {
                MotParam[i] = new TMotionItem();
            }
        }

        public void CopyFrom(in TMotionParam arg)
        {
            strLogHead = arg.strLogHead;

            if (MotParam == null || MotParam.Length != arg.MotParam.Length)
                MotParam = new TMotionItem[(int)eMtr.MAX_SERVO_AXIS];

            for (int i = 0; i < MotParam.Length; i++)
            {
                if (MotParam[i].MotionUnit == null)
                    MotParam[i] = new TMotionItem();

                MotParam[i].CopyFrom(arg.MotParam[i]);
            }
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
                //TODO: Update load motion param function
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
                //TODO: Update save motion param function
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public void SavePosValue(string filePath, int motorIndex)
        {
            //TODO: Update save position function
        }
        public double GetMaxLimit(int nMotor) => MotParam[nMotor].MaxLimit;
        public double GetMinLimit(int nMotor) => MotParam[nMotor].MinLimit;
        public double GetPosition(int nMotor, int nPosId) => MotParam[nMotor].GetPosition(nPosId);
        public double GetVelocity(int nMotor, int nPosId) => MotParam[nMotor].GetVelocity(nPosId);
        public double GetAccel(int nMotor, int nPosId) => MotParam[nMotor].GetAccel(nPosId);
    }
}
