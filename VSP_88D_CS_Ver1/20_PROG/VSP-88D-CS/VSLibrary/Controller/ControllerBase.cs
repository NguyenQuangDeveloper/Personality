using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSLibrary.Controller
{
    /// <summary>
    /// 아날로그 I/O 컨트롤러의 기본 추상 클래스입니다.
    /// 이 클래스는 IAnalogIOController 인터페이스를 구현하며, 아날로그 I/O 장치의 데이터 관리,
    /// 채널 읽기/쓰기 및 디바이스 목록 반환 기능을 위한 추상 메서드를 정의합니다.
    /// 파생 클래스에서 구체적인 동작을 구현해야 합니다.
    /// </summary>
    public abstract class AIOBase : IAIOBase
    {
        public abstract Dictionary<string, IAnalogIOData> GetAnalogIODataDictionary();

        public virtual void AnalogIOCtrlDispose() { }

        public virtual void MonitorAnalogIOController() { }

        public abstract double ReadChannelValue(IAnalogIOData AioData);

        public abstract bool WriteChannelValue(IAnalogIOData AioData, double value);

        public abstract void UpdateAllChannelValues();
    }

    public abstract class AIODataBase : ViewModelBase, IAnalogIOData
    {
        public abstract IAIOBase Controller { get; set; }
        public abstract ControllerType ControllerType { get; set; }
        public abstract IOType IOType { get; set; }
        public abstract short ModuleNumber { get; set; }
        public abstract string WireName { get; set; }
        public abstract string EmName { get; set; }
        public abstract string StrdataName { get; set; }
        public abstract string ModuleName { get; set; }
        public abstract int Channel { get; set; }
        public abstract int Range { get; set; }
        public abstract double AValue { get; set; }        

        public virtual double ReadValue(bool forceRead = false)
        {
            if (Controller == null)
                MessageBox.Show("컨트롤러가 연결되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            if (forceRead)
            {
                // 컨트롤러에서 강제로 다시 읽기
                AValue = Controller!.ReadChannelValue(this);
            }

            return AValue;
        }
        public virtual void WriteValue(double value)
        {
            if (IOType != IOType.OUTPut)
                MessageBox.Show("이 채널은 출력 전용입니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            //throw new InvalidOperationException("이 채널은 출력 전용입니다.");

            if (Controller == null)
                MessageBox.Show("컨트롤러가 연결되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            //throw new InvalidOperationException("컨트롤러가 연결되지 않았습니다.");

            Controller!.WriteChannelValue(this, value);
            AValue = value;
        }
    }

    /// <summary>
    /// 디지털 I/O 컨트롤러의 기본 추상 클래스입니다.
    /// 이 클래스는 IDigitalIOController 인터페이스를 구현하며, 공통 기능의 기본 구조를 제공합니다.
    /// </summary>
    public abstract class DIOBase : IDIOBase
    {

        // ==================== 디지털 I/O 컨트롤러 기능 ====================

        public abstract Dictionary<string, IDigitalIOData> GetDigitalIODataDictionary();

        public virtual void DigitalIOCtrlDispose() { }

        public virtual void MonitorDigitalIOController() { }

        public abstract bool ReadBit(IDigitalIOData dioData);

        public abstract bool WriteBit(IDigitalIOData dioData, bool value);

        public abstract void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key);

        public abstract void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value);

        public abstract void UpdateAllIOStates();
    }

    public abstract class DIODataBase : ViewModelBase, IDigitalIOData
    {
        public abstract IDIOBase Controller { get; set; }
        public abstract short AxisNo { get; set; }
        public abstract ControllerType ControllerType { get; set; }
        public abstract IOType IOType { get; set; }
        public abstract string WireName { get; set; }
        public abstract string EmName { get; set; }
        public abstract string StrdataName { get; set; }
        public abstract string ModuleName { get; set; }
        public abstract int ModuleIndex { get; set; }
        public abstract bool Value { get; set; }
        public abstract bool PollingState { get; set; }
        public abstract bool StateReversal { get; set; }
        public abstract int Offset { get; set; }
        public abstract bool Edge { get; set; }
        public abstract int DetectionTime { get; set; }

        //컨트롤
        public virtual bool IsOn(bool forceRead = false)
        {
            if (Controller == null)
                MessageBox.Show("DIO 컨트롤러가 연결되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            //throw new InvalidOperationException("DIO 컨트롤러가 연결되지 않았습니다.");

            if (forceRead)
                Value = Controller!.ReadBit(this);

            return Value;
        }

        public virtual bool IsOff(bool forceRead = false)
        {
            return !IsOn(forceRead);
        }

        public virtual void On()
        {
            if (IOType != IOType.OUTPut)
                MessageBox.Show("해당 채널은 출력 전용이 아닙니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            if (Controller == null)
                MessageBox.Show("DIO 컨트롤러가 연결되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            Controller!.WriteBit(this, true);
        }

        public virtual void Off()
        {
            if (IOType != IOType.OUTPut)
                MessageBox.Show("해당 채널은 출력 전용이 아닙니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            if (Controller == null)
                MessageBox.Show("DIO 컨트롤러가 연결되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            //throw new InvalidOperationException("DIO 컨트롤러가 연결되지 않았습니다.");

            Controller!.WriteBit(this, false);
        }
    }

    /// <summary>
    /// 다축 모션 컨트롤러의 기본 추상 클래스 (유니버셜 I/O 포함).
    /// 이 클래스는 <see cref="AbstractDigitalIOController"/>와 <see cref="IMotionController"/>를 상속받아,
    /// 모션 제어와 관련된 다양한 동작(이동, 정지, 홈 이동, 서보 제어, 파라미터 관리, I/O 제어 등)을 추상적으로 정의합니다.
    /// 파생 클래스는 이 클래스에서 선언된 추상 메서드들을 구체적으로 구현해야 합니다.
    /// </summary>
    public abstract class MotionBase : IMotionBase
    {
        // ==================== 다축 모션 컨트롤러 기능 ====================       

        public abstract Dictionary<int, IAxisData> GetMotionDataDictionary();  

        public virtual void MotionCtrlDispose() { }

        public abstract void MoveToPosition(int axis, double position, double velocity, double acceleration);

        public abstract void MoveToPoint(int axis, double position, double velocity, double acceleration);

        public abstract void Repeat(int axis, double[] position, double velocity, double acceleration, int repeatCount);

        public abstract bool StopMotion(int axis);

        public abstract double GetPosition(int axis);
        
        public abstract double GetCmdPosition(int axis);

        public abstract double GetVelocity(int axis);

        public abstract void SetServoOnOff(int axis, bool enabled);

        public abstract bool SetParameter(IAxisData motionData);

        public abstract bool GetParameter(IAxisData motionData);

        public abstract bool IsServo(int axis);

        public abstract bool IsHomed(int axis);

        public abstract Task ClearAlarm(int axis);

        public abstract Task HomeMove(int axis, Motion_HomeConfig initset);

        public abstract bool IsAlarm(int axis);

        public abstract bool IsMoving(int axis);

        public abstract bool IsPositiveLimit(int axis);

        public abstract bool IsNegativeLimit(int axis);

        public abstract void UpdateAllIOStatus();

        public abstract void UpdateAllPosition();

        // ==================== 유니버셜 I/O 기능 ====================

        public abstract bool SetOutput(int axis, int port, bool state);

        public abstract bool GetInput(int axis, int port);

        public abstract bool GetOutput(int axis, int port);
    }

    public abstract class AxisDataBase : ViewModelBase, IAxisData
    {
        // ───────────────────────────────────────────────
        // 기본 정보
        // ───────────────────────────────────────────────
        public abstract IMotionBase Controller { get; set; }

        //기본모션상태
        public abstract bool ServoEnabled { get; set; } //서보상태
        public abstract bool HomeState { get; set; } //홈상태
        public abstract bool Alarm { get; set; } //알람
        public abstract bool PositiveLimit { get; set; } //+리미트
        public abstract bool NegativeLimit { get; set; } //-리미트
        public abstract bool InPosition { get; set; } //인포지션

        // ───────────────────────────────────────────────
        // 명령 위치 및 속도, 가속도  현재 위치 및 속도
        // ───────────────────────────────────────────────
        
        //명령 위치,속도,가속도
        public abstract double CurrentPosition { get; set; }
        public abstract double CurrentVelocity { get; set; }
        public abstract double CurrentAcceleration { get; set; }

        //현재 위치,속도
        public abstract double Position { get; set; }
        public abstract double Velocity { get; set; }

        // ───────────────────────────────────────────────
        // 셋팅 파라미터
        // ───────────────────────────────────────────────

        //기본파라미터
        public abstract ControllerType ControllerType { get; set; }
        public abstract short AxisNo { get; set; }
        public abstract string AxisName { get; set; }
        public abstract string StrAxisData { get; set; }

        //이동거리 계산에 필요한 변수
        public abstract double LeadPitch { get; set; } // 가속도 볼스크류 피치 (mm/회전)
        public abstract double PulsesPerRev { get; set; } // 모터 1회전당 펄스 수 (pulse/rev)
        public abstract double GearRatio { get; set; } // 기어비 (입력 회전수 대비 출력 회전수)

        public abstract byte PulseOutputMode { get; set; } // 펄스 출력 방식*
        public abstract byte EncInputMode { get; set; } // 엔코더 입력 방식*

        //속도 제한
        public abstract double MinSpeed { get; set; }
        public abstract double MaxSpeed { get; set; }

        // 서보 모터 활성화 반전 여부
        public abstract bool ServoEnabledReversal { get; set; }

        //레벨 설정 셋팅
        public abstract bool LvSet_EndLimitP { get; set; } // 양(+) 방향 엔드 리미트
        public abstract bool LvSet_EndLimitN { get; set; } // 음(-) 방향 엔드 리미트
        public abstract bool LvSet_SlowLimitP { get; set; } // 양(+) 방향 슬로우 리미트
        public abstract bool LvSet_SlowLimitN { get; set; } // 음(-) 방향 슬로우 리미트
        public abstract bool LvSet_InPosition { get; set; } // 위치결정 완료 신호
        public abstract bool LvSet_Alarm { get; set; } // 축의 알람 상태

        // ───────────────────────────────────────────────
        // 제어 함수
        // ───────────────────────────────────────────────
        public virtual void MoveToPosition(double position, double velocity, double acceleration)
            => Controller?.MoveToPosition(AxisNo, position, velocity, acceleration);

        public virtual void MoveToPoint(double position, double velocity, double acceleration)
            => Controller?.MoveToPoint(AxisNo, position, velocity, acceleration);

        public virtual void Repeat(double[] positions, double velocity, double acceleration, int repeatCount)
            => Controller?.Repeat(AxisNo, positions, velocity, acceleration, repeatCount);

        public virtual bool StopMotion()
            => Controller?.StopMotion(AxisNo) ?? false;
        public virtual bool SetServoOnOff(bool enabled)
        {
            Controller?.SetServoOnOff(AxisNo, enabled);
            return enabled;
        }
        public virtual Task HomeMove(Motion_HomeConfig homeConfig)
            => _ = Controller?.HomeMove(AxisNo, homeConfig) ?? Task.CompletedTask;

        // ───────────────────────────────────────────────
        // 상태 확인 및 IO
        // 드라이버단에서 직접 데이터를 가져오는 함수        
        // ───────────────────────────────────────────────   
        public virtual double GetPosition() => Controller?.GetPosition(AxisNo) ?? 0;
        public virtual double GetCmdPosition() => Controller?.GetCmdPosition(AxisNo) ?? 0;
        public virtual double GetVelocity() => Controller?.GetVelocity(AxisNo) ?? 0;

        public virtual bool IsServo() => Controller?.IsServo(AxisNo) ?? false;
        public virtual bool IsHomed() => Controller?.IsHomed(AxisNo) ?? false;      
        public virtual bool IsAlarm() => Controller?.IsAlarm(AxisNo) ?? false;
        public virtual bool IsMoving() => Controller?.IsMoving(AxisNo) ?? false;
        public virtual bool IsPositiveLimit() => Controller?.IsPositiveLimit(AxisNo) ?? false;
        public virtual bool IsNegativeLimit() => Controller?.IsNegativeLimit(AxisNo) ?? false;
        
        public virtual bool ClearAlarm()
        {
            Controller?.ClearAlarm(AxisNo);
            return true;
        }

        // ───────────────────────────────────────────────
        // 파라미터 저장/복원
        // ───────────────────────────────────────────────
        public virtual bool GetParameter() => Controller?.GetParameter(this) ?? false;
        public virtual bool SetParameter(IAxisData motionData)
            => Controller?.SetParameter(motionData) ?? false;
    }
}
