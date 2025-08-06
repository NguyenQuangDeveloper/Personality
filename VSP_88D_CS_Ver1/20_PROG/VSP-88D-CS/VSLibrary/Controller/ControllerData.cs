using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSLibrary.Controller
{
    /// <summary>
    /// 모션 제어 옵션을 정의하는 클래스입니다. 각 속성은 축 제어에 필요한 다양한 설정 값을 나타냅니다.
    /// </summary>
    public class AIOData : AIODataBase
    {
        public override IAIOBase Controller { get; set; } = null!;

        //인터페이스 공통 사용
        //#####################################
        public ControllerType _ControllerType;
        public override ControllerType ControllerType
        {
            get => _ControllerType;
            set => _ControllerType = value;
        }

        public IOType _iOType;
        public override IOType IOType
        {
            get => _iOType;
            set => _iOType = value;
        }

        public override short ModuleNumber { get; set; } // 축 번호   
        public override string WireName { get; set; } = string.Empty;   //와이어번호
        public override string EmName { get; set; } = string.Empty;
        public override string StrdataName { get; set; } = string.Empty;  //IO 이름
        public override string ModuleName { get; set; } = string.Empty;//모듈의 이름 (예: SIO_DO32P)
        public override int Channel { get; set; } //입력 또는 출력의 오프셋 (특정 비트를 지정하는 값)
        public override int Range { get; set; }
        
        private double _value = 0;
        
        [NotMapped] // Entity Framework에서 제외
        public override double AValue
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

    /// <summary>
    /// 모션 제어 옵션을 정의하는 클래스입니다.
    /// 각 속성은 축 제어에 필요한 다양한 설정 값을 나타내며, 디지털 I/O 데이터와 관련된 정보를 포함합니다.
    /// </summary>
    public class DIOData : DIODataBase
    {
        public override IDIOBase Controller { get; set; } = null!;

        public ControllerType _controllerType;        
        public override ControllerType ControllerType
        {
            get => _controllerType;
            set => _controllerType = value;
        }

        public IOType _iOType;
        public override IOType IOType
        {
            get => _iOType;
            set => _iOType = value;
        }

        public override short AxisNo { get; set; }

        public override string WireName { get; set; } = string.Empty;

        public override string EmName { get; set; } = string.Empty;

        private string _strdataName = string.Empty;
        public override string StrdataName
        {
            get => _strdataName;
            set => SetProperty(ref _strdataName, value);
        }
        
        public override string ModuleName { get; set; } = string.Empty;

        public override int ModuleIndex { get; set; }

        public override bool PollingState { get; set; }

        public override bool StateReversal { get; set; } = false;

        public override int Offset { get; set; }

        public override bool Edge { get; set; } = false;

        public override int DetectionTime { get; set; } = 0;

        private bool _value = false;

        [NotMapped]
        public override bool Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

    /// <summary>
    /// 홈설정
    /// </summary>
    public struct Motion_HomeConfig
    {
        public double[] HomeVelocities { get; set; } // 홈속도
        public double[] HomeAccelerations { get; set; } // 홈가속도
        public double[] HomeOffsets { get; set; } // 홈오프셋
                                                  //public byte[] HomeMethods { get; set; } // 홈오프셋
        public bool[] HomeDirection { get; set; }
        // 신호 검출 구동-====================================================================================================
        // 지정 신호의 상향/하향 에지를 검색하여 급정지 또는 감속정지를 할 수 있다.
        // detect_signal : 검색 신호 설정(typedef : DETECT_DESTINATION_SIGNAL)
        // PElmNegativeEdge    = 0x0,        // +Elm(End limit) 하강 edge
        // NElmNegativeEdge    = 0x1,        // -Elm(End limit) 하강 edge
        // PSlmNegativeEdge    = 0x2,        // +Slm(Slowdown limit) 하강 edge
        // NSlmNegativeEdge    = 0x3,        // -Slm(Slowdown limit) 하강 edge
        // In0DownEdge         = 0x4,        // IN0(ORG) 하강 edge
        // In1DownEdge         = 0x5,        // IN1(Z상) 하강 edge
        // In2DownEdge         = 0x6,        // IN2(범용) 하강 edge
        // In3DownEdge         = 0x7,        // IN3(범용) 하강 edge
        // PElmPositiveEdge    = 0x8,        // +Elm(End limit) 상승 edge
        // NElmPositiveEdge    = 0x9,        // -Elm(End limit) 상승 edge
        // PSlmPositiveEdge    = 0xa,        // +Slm(Slowdown limit) 상승 edge
        // NSlmPositiveEdge    = 0xb,        // -Slm(Slowdown limit) 상승 edge
        // In0UpEdge           = 0xc,        // IN0(ORG) 상승 edge
        // In1UpEdge           = 0xd,        // IN1(Z상) 상승 edge
        // In2UpEdge           = 0xe,        // IN2(범용) 상승 edge
        // In3UpEdge           = 0xf         // IN3(범용) 상승 edge
        // Signal Search1 : 구동 시작후 입력 속도까지 가속하여, 신호 검출후 감속 정지.
        // Signal Search2 : 구동 시작후 가속없이 입력 속도가 되고, 신호 검출후 급정지. 
        // 주의 : Signal Search2는 가감속이 없으므로 속도가 높을경우 탈조및 기구부의 무리가 갈수 있으므로 주의한다.
        // *s*_*    : 구동중 속도 프로파일을 "S curve"를 이용한다. "*s_*"가 없다면 사다리꼴 가감속을 이용한다.
        // *_ex     : 구동중 가감속도를 가속 또는 감속 시간으로 입력 받는다. "*_ex"가 없다면 가감속률로 입력 받는다.
        public byte[] HomeDetectSignal { get; set; } // 홈오프셋 

        public Motion_HomeConfig(double[] velocities, double[] accelerations, double[] offsets, bool[] Direction, byte[] HDetectSignal)
        {
            HomeVelocities = velocities;
            HomeAccelerations = accelerations;
            HomeOffsets = offsets;
            HomeDirection = Direction;
            HomeDetectSignal = HDetectSignal;
        }
    }

    /// Mechanical Signal 상태를 세부적으로 표현하는 구조체
    /// </summary>
    public struct Motion_MechanicalSignal
    {
        public bool MODE8_16 { get; set; }      // 14비트: MODE8_16 신호 입력 Level
        public bool SYNC { get; set; }          // 13비트: SYNC 신호 입력 Level
        public bool ESTOP { get; set; }         // 12비트: ESTOP 신호 입력 Level
        public bool SSTOP { get; set; }         // 11비트: SSTOP 신호 입력 Level
        public bool MARK { get; set; }          // 10비트: MARK 신호 입력 Level
        public bool EXPP_MPG { get; set; }      // 9비트: EXPP(MPG) 신호 입력 Level
        public bool EXMP_MPG { get; set; }      // 8비트: EXMP(MPG) 신호 입력 Level
        public bool EncoderUp { get; set; }     // 7비트: Encoder Up신호 입력 Level(A상 신호)
        public bool EncoderDown { get; set; }   // 6비트: Encoder Down신호 입력 Level(B상 신호)
        public bool INPOSITION { get; set; }    // 5비트: INPOSITION 신호 Active 상태
        public bool ALARM { get; set; }         // 4비트: ALARM 신호 Active 상태
        public bool NLimitStop { get; set; }    // 3비트: -Limit 감속정지 신호 Active 상태
        public bool PLimitStop { get; set; }    // 2비트: +Limit 감속정지 신호 Active 상태
        public bool NLimitEStop { get; set; }   // 1비트: -Limit 급정지 신호 Active 상태
        public bool PLimitEStop { get; set; }   // 0비트: +Limit 급정지 신호 Active 상태

        public Motion_MechanicalSignal(uint signal)
        {
            MODE8_16 = (signal & (1u << 14)) != 0;
            SYNC = (signal & (1u << 13)) != 0;
            ESTOP = (signal & (1u << 12)) != 0;
            SSTOP = (signal & (1u << 11)) != 0;
            MARK = (signal & (1u << 10)) != 0;
            EXPP_MPG = (signal & (1u << 9)) != 0;
            EXMP_MPG = (signal & (1u << 8)) != 0;
            EncoderUp = (signal & (1u << 7)) != 0;
            EncoderDown = (signal & (1u << 6)) != 0;
            INPOSITION = (signal & (1u << 5)) != 0;
            ALARM = (signal & (1u << 4)) != 0;
            NLimitStop = (signal & (1u << 3)) != 0;
            PLimitStop = (signal & (1u << 2)) != 0;
            NLimitEStop = (signal & (1u << 1)) != 0;
            PLimitEStop = (signal & (1u << 0)) != 0;
        }
    }    

    /// <summary>
    /// IO 입력 및 출력 상태를 나타내는 구조체입니다.
    /// </summary>
    public struct Motion_IOStatus
    {
        public uint InputStatus { get; set; }  // 입력 상태 (비트 값)
        public uint OutputStatus { get; set; } // 출력 상태 (비트 값)
        public bool Input_0 { get; set; }  // Home 입력0
        public bool Input_1 { get; set; }  // Z Phase 입력1
        public bool Input_2 { get; set; }  // 입력2
        public bool Input_3 { get; set; }  // 입력3
        public bool Input_4 { get; set; }  // 입력4
        public bool Input_5 { get; set; }  // 입력5

        public bool Output_0 { get; set; }  // Servo On
        public bool Output_1 { get; set; }  // 알람클리어
        public bool Output_2 { get; set; }  // 출력2
        public bool Output_3 { get; set; }  // 출력3
        public bool Output_4 { get; set; }  // 출력4
        public bool Output_5 { get; set; }  // 출력5

        public Motion_IOStatus(uint inputStatus, uint outputStatus)
        {
            InputStatus = inputStatus;
            OutputStatus = outputStatus;

            Input_0 = (inputStatus & (1u << 0)) != 0;
            Input_1 = (inputStatus & (1u << 1)) != 0;
            Input_2 = (inputStatus & (1u << 2)) != 0;
            Input_3 = (inputStatus & (1u << 3)) != 0;
            Input_4 = (inputStatus & (1u << 4)) != 0;
            Input_5 = (inputStatus & (1u << 5)) != 0;

            Output_0 = (outputStatus & (1u << 0)) != 0;
            Output_1 = (outputStatus & (1u << 1)) != 0;
            Output_2 = (outputStatus & (1u << 2)) != 0;
            Output_3 = (outputStatus & (1u << 3)) != 0;
            Output_4 = (outputStatus & (1u << 4)) != 0;
            Output_5 = (outputStatus & (1u << 5)) != 0;
        }
    }

    /// <summary>
    /// 모션 제어 옵션을 정의하는 클래스입니다. 각 속성은 축 제어에 필요한 다양한 설정 값을 나타냅니다.
    /// </summary>
    public class AxtAxisData : AxisDataBase
    {
        // ───────────────────────────────────────────────
        // 기본 정보
        // ───────────────────────────────────────────────
        public override IMotionBase Controller { get; set; } = null!;

        //기본모션상태
        private bool _servoEnabled; // 실제 저장하는 필드
        public override bool ServoEnabled
        {
            get => _servoEnabledReversal ? !_servoEnabled : _servoEnabled;
            set
            {
                var processedValue = _servoEnabledReversal ? !value : value;
                SetProperty(ref _servoEnabled, processedValue);
            }
        }

        private bool _homeState; //홈상태
        public override bool HomeState { get => _homeState; set => SetProperty(ref _homeState, value); }
        private bool _alarm;  //알람
        public override bool Alarm { get => _alarm; set => SetProperty(ref _alarm, value); }
        private bool _positiveLimit; //+리미트
        public override bool PositiveLimit { get => _positiveLimit; set => SetProperty(ref _positiveLimit, value); }
        private bool _negativeLimit; //-리미트
        public override bool NegativeLimit { get => _negativeLimit; set => SetProperty(ref _negativeLimit, value); }
        private bool _inPosition; //인포지션
        public override bool InPosition { get => _inPosition; set => SetProperty(ref _inPosition, value); }

        // ───────────────────────────────────────────────
        // 명령 위치 및 속도, 가속도  현재 위치 및 속도
        // ───────────────────────────────────────────────
        private double _currentPosition; // 목표 위치
        public override double CurrentPosition { get => _currentPosition; set => SetProperty(ref _currentPosition, value); }
        private double _currentVelocity; // 속도 (mm/s)
        public override double CurrentVelocity { get => _currentVelocity; set => SetProperty(ref _currentVelocity, value); }
        private double _currentAcceleration; // 가속도 (mm/s²)
        public override double CurrentAcceleration { get => _currentAcceleration; set => SetProperty(ref _currentAcceleration, value); }

        private double _position; // 현재 위치
        public override double Position { get => _position; set => SetProperty(ref _position, value); }
        private double _velocity; // 속도 (mm/s)
        public override double Velocity { get => _velocity; set => SetProperty(ref _velocity, value); }

        // ───────────────────────────────────────────────
        // 셋팅 파라미터
        // ───────────────────────────────────────────────

        //기본파라미터
        public override short AxisNo { get; set; } // 축 번호
        public override string AxisName { get; set; } = string.Empty; // 축의 이름
        public override string StrAxisData { get; set; } = string.Empty; // 축의 위치 명시

        public ControllerType _controller;
        public override ControllerType ControllerType
        {
            get => _controller;
            set => _controller = value;
        }

        //이동거리 계산에 필요한 변수
        public override double LeadPitch { get; set; } // 가속도 볼스크류 피치 (mm/회전)
        public override double PulsesPerRev { get; set; } // 모터 1회전당 펄스 수 (pulse/rev)
        public override double GearRatio { get; set; } // 기어비 (입력 회전수 대비 출력 회전수)

        //속도 제한
        public override double MinSpeed { get; set; } // 시작 속도 (Unit/sec)
        public override double MaxSpeed { get; set; } // 최대 속도 (Unit/sec)

        // OneHighLowHigh   = 0x0, 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
        // OneHighHighLow   = 0x1, 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
        // OneLowLowHigh    = 0x2, 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
        // OneLowHighLow    = 0x3, 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
        // TwoCcwCwHigh     = 0x4, 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High 
        // TwoCcwCwLow      = 0x5, 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low 
        // TwoCwCcwHigh     = 0x6, 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
        // TwoCwCcwLow      = 0x7, 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
        public override byte PulseOutputMode { get; set; } // 펄스 출력 방식*

        // UpDownMode = 0x0    // Up/Down
        // Sqr1Mode   = 0x1    // 1체배
        // Sqr2Mode   = 0x2    // 2체배
        // Sqr4Mode   = 0x3    // 4체배
        public override byte EncInputMode { get; set; } // 엔코더 입력 방식*

        // 서보 모터 활성화 반전 여부
        private bool _servoEnabledReversal = false; 
        public override bool ServoEnabledReversal
        {
            get => _servoEnabledReversal;
            set => _servoEnabledReversal = value;
        }

        //레벨 설정 셋팅
        private bool _lvSet_EndLimitP; // 양(+) 방향 엔드 리미트
        public override bool LvSet_EndLimitP { get => _lvSet_EndLimitP; set => SetProperty(ref _lvSet_EndLimitP, value); }
        private bool _lvSet_EndLimitN; // 음(-) 방향 엔드 리미트
        public override bool LvSet_EndLimitN { get => _lvSet_EndLimitN; set => SetProperty(ref _lvSet_EndLimitN, value); }
        private bool _lvSet_SlowLimitP; // 양(+) 방향 슬로우 리미트
        public override bool LvSet_SlowLimitP { get => _lvSet_SlowLimitP; set => SetProperty(ref _lvSet_SlowLimitP, value); }
        private bool _lvSet_SlowLimitN; // 음(-) 방향 슬로우 리미트
        public override bool LvSet_SlowLimitN { get => _lvSet_SlowLimitN; set => SetProperty(ref _lvSet_SlowLimitN, value); }
        private bool _lvSet_InPosition; // 위치결정 완료 신호
        public override bool LvSet_InPosition { get => _lvSet_InPosition; set => SetProperty(ref _lvSet_InPosition, value); }
        private bool _lvSet_Alarm; // 축의 알람 상태
        public override bool LvSet_Alarm { get => _lvSet_Alarm; set => SetProperty(ref _lvSet_Alarm, value); }

        // ───────────────────────────────────────────────
        // 제어 함수
        // ───────────────────────────────────────────────
        // AxisDataBase 클래스 상속

        // ───────────────────────────────────────────────
        // 상태 확인 및 IO
        // ───────────────────────────────────────────────
        // AxisDataBase 클래스 상속

        // ───────────────────────────────────────────────
        // 파라미터 저장/복원
        // ───────────────────────────────────────────────
        // AxisDataBase 클래스 상속

        // ───────────────────────────────────────────────
        // AXT 필드
        // ───────────────────────────────────────────────

        public int BoardNo { get; set; } // 보드 번호
        public int ModuleID { get; set; } // 모듈 ID
        public string ModuleName { get; set; } = string.Empty; // 모듈 이름
        public double MoveUnitPerPulse { get; set; } // 펄스당 이동 단위          

        //유니버셜 IO
        public Motion_IOStatus IOStatus { get; set; }

        //원점 검색 관련 필드
        public Motion_HomeConfig Motion_HomeCfg { get; set; }

        // Mechanical Signal 상태를 세부적으로 저장
        public Motion_MechanicalSignal MechanicalSignal { get; set; } // 세분화된 Mechanical Signal 상태

        public AutoResetEvent _homeSearchCompletedEvent = new AutoResetEvent(false); // 원점 검색 완료 이벤트

        public bool _stopRequested = false; // 정지 요청 플래그        
    }
}
