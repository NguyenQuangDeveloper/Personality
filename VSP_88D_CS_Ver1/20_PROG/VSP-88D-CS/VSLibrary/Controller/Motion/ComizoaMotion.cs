using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Controller.DigitalIO;

namespace VSLibrary.Controller.Motion
{
    public class ComizoaMotion : MotionBase
    {
        public override Task ClearAlarm(int axis)
        {
            throw new NotImplementedException();
        }

        public void DigitalIOCtrlDispose()
        {
            throw new NotImplementedException();
        }

        public override double GetCmdPosition(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool GetInput(int axis, int port)
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<string> GetIODevicelist()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<int, IAxisData> GetMotionDataDictionary()
        {
            throw new NotImplementedException();
        }

        public override bool GetOutput(int axis, int port)
        {
            throw new NotImplementedException();
        }

        public override bool GetParameter(IAxisData motionData)
        {
            throw new NotImplementedException();
        }

        public override bool SetParameter(IAxisData motionData)
        {
            throw new NotImplementedException();
        }

        public override double GetPosition(int axis)
        {
            throw new NotImplementedException();
        }

        public override double GetVelocity(int axis)
        {
            throw new NotImplementedException();
        }

        public override Task HomeMove(int axis, Motion_HomeConfig initset)
        {
            throw new NotImplementedException();
        }

        public override bool IsAlarm(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool IsHomed(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool IsMoving(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool IsNegativeLimit(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool IsPositiveLimit(int axis)
        {
            throw new NotImplementedException();
        }

        public override bool IsServo(int axis)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPoint(int axis, double position, double velocity, double acceleration)
        {
            throw new NotImplementedException();
        }

        public override void MoveToPosition(int axis, double position, double velocity, double acceleration)
        {
            throw new NotImplementedException();
        }

        public bool ReadBit(IDigitalIOData dioData)
        {
            throw new NotImplementedException();
        }

        public void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key)
        {
            throw new NotImplementedException();
        }

        public override void Repeat(int axis, double[] position, double velocity, double acceleration, int repeatCount)
        {
            throw new NotImplementedException();
        }

        public override bool SetOutput(int axis, int port, bool state)
        {
            throw new NotImplementedException();
        }

        public override void SetServoOnOff(int axis, bool enabled)
        {
            throw new NotImplementedException();
        }

        public override bool StopMotion(int axis)
        {
            throw new NotImplementedException();
        }

        public void UpdateAllIOStates()
        {
            throw new NotImplementedException();
        }

        public override void UpdateAllPosition()
        {
            throw new NotImplementedException();
        }

        public override void UpdateAllIOStatus()
        {
            throw new NotImplementedException();
        }

        public bool WriteBit(IDigitalIOData dioData, bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value)
        {
            throw new NotImplementedException();
        }
    }
}
