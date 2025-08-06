using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace VSLibrary.Controller.Motion
{
    public class AxtMotion : MotionBase, IDIOBase
    {
        private Dictionary<int, AxtAxisData> _axisData = new Dictionary<int, AxtAxisData>();

        private Dictionary<string, IDigitalIOData> _universalIOData = new Dictionary<string, IDigitalIOData>();        

        private Dictionary<string, int> _iocount;

        private bool _isInitialized = false;  // 모션 컨트롤러 초기화 여부

        /// <summary>
        /// Initializes a new instance of the <see cref="AxtMotion"/> class
        /// using the provided I/O count offsets.
        /// Generates IoType data and attempts to open the device if initialization succeeds.
        /// </summary>
        /// <param name="count">Dictionary of I/O count offsets.</param>
        public AxtMotion(Dictionary<string, int> count)
        {
            _iocount = count;

            SetupTestData();
            
            if (IsInitialized())
            {
                OpenDevice();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Validates that data for the specified axis exists.
        /// </summary>
        /// <param name="axis">Axis index to check.</param>
        /// <returns>True if axis data exists; otherwise, false.</returns>
        private bool CheckDic(int axis)
        {
            if ((!_axisData.ContainsKey(axis)))// || _isInitialized == false) //디비 데이터를 보내야 하기 때문에 주석처리
            {
                Console.WriteLine($"[오류] 축 {axis}의 데이터가 존재하지 않거나 초기화 되지 않았습니다.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Populates IoType axis and universal I/O data for simulation.
        /// Increments the I/O count offsets after generation.
        /// </summary>
        private void SetupTestData()
        {
            int inputCounter = 0, outputCounter = 0;
            //초기화
            for (int i = 0; i < 15; i++)
            {
                _axisData.Add(i, new AxtAxisData
                {
                    Controller = this,
                    AxisName = $"MT{i:00}",
                    AxisNo = (short)i
                });

                //I/O의 각 6개씩만큼 추가
                for (int j = 0; j < 6; j++)
                {
                    _universalIOData.Add($"MX{inputCounter + _iocount["MDInput"]:X3}", new DIOData
                    {
                        Controller = this,
                        WireName = $"MX{inputCounter + _iocount["MDInput"]:X3}",
                        IOType = IOType.InPut,
                        ControllerType = ControllerType.DIO_AjinAXTUniversalIO,
                        AxisNo = (short)i,
                        ModuleIndex = j,
                    });
                    inputCounter++;
                }
                

                for (int j = 0; j < 6; j++)
                {
                    _universalIOData.Add($"MY{outputCounter + _iocount["MDOutput"]:X3}", new DIOData
                    {
                        Controller = this,
                        WireName = $"MY{outputCounter + _iocount["MDOutput"]:X3}",
                        IOType = IOType.OUTPut,
                        ControllerType = ControllerType.DIO_AjinAXTUniversalIO,
                        AxisNo = (short)i,
                        ModuleIndex = j,
                    });
                    outputCounter++;
                }                
            }

            _iocount["MDInput"] = _iocount["MDInput"] + inputCounter;
            _iocount["MDOutput"] = _iocount["MDOutput"] + outputCounter;

            //_isInitialized = true;
        }

        /// <summary>
        /// Verifies that the required CAxtCAMCFS20 DLLs are present in the specified path.
        /// </summary>
        /// <param name="path">Directory path to check.</param>
        /// <param name="requiredDlls">List of required DLL filenames.</param>
        /// <returns>True if all DLLs are found; otherwise, false.</returns>
        private bool CheckDllPath(string path, string[] requiredDlls)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"❌ DLL 경로가 존재하지 않음: {path}");
                return false;
            }

            foreach (var dll in requiredDlls)
            {
                var fullPath = Path.Combine(path, dll);
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"❌ 누락된 DLL: {dll} → {fullPath}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to initialize the CAxtCAMCFS20 module if not already initialized.
        /// </summary>
        /// <returns>True if initialization succeeded or was already done; otherwise, false.</returns>
        private bool IsInitialized()
        {
            if (CAxtCAMCFS20.CFS20IsInitialized() == 0)
            {
                // CAxtCAMCFS20 모듈 초기화 시도
                int result = CAxtCAMCFS20.InitializeCAMCFS20(1);
                if (result == 0)
                {
                    // 초기화 실패 시 연결 플래그를 false로 설정
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Updates all I/O statuses and mechanical signals for each axis.
        /// </summary>
        public override void UpdateAllIOStatus()
        {
            if (!_isInitialized) return;
            
            foreach (AxtAxisData motionData in _axisData.Values)
            {
                // IO 상태 가져오기 (MotionIOManager 활용)
                motionData.IOStatus = GetIOStatus(motionData, _universalIOData);

                // 서보 상태 업데이트 (서보 활성화 반전 값 적용)
                motionData.ServoEnabled = motionData.ServoEnabledReversal ? !motionData.IOStatus.Output_0 : motionData.IOStatus.Output_0;
                if (!motionData.ServoEnabled)
                    motionData.HomeState = false;

                // 메카니컬 신호 상태 업데이트
                ushort signal = CAxtCAMCFS20.CFS20get_mechanical_signal(motionData.AxisNo);
                
                motionData.MechanicalSignal = new Motion_MechanicalSignal(signal);

                motionData.Alarm = motionData.MechanicalSignal.ALARM;
                motionData.PositiveLimit = motionData.MechanicalSignal.PLimitEStop;
                motionData.NegativeLimit = motionData.MechanicalSignal.NLimitEStop;
                motionData.InPosition = motionData.MechanicalSignal.INPOSITION;
            }
        }

        /// <summary>
        /// Reads raw input/output bits via CAxtCAMCFS20 API and maps them to DIOData items.
        /// </summary>
        private Motion_IOStatus GetIOStatus(AxtAxisData motionData, Dictionary<string, IDigitalIOData> iodata)
        {
            uint inputStatus = 0;
            uint outputStatus = 0;

            // 지정된 축에 대해 현재 입력 및 출력 상태를 가져옵니다.
            inputStatus = CAxtCAMCFS20.CFS20get_input(motionData.AxisNo);
            outputStatus = CAxtCAMCFS20.CFS20get_output(motionData.AxisNo);

            foreach (DIOData item in iodata.Values)
            {
                if (item.AxisNo == motionData.AxisNo)
                {
                    if (item.IOType == IOType.InPut)
                    {
                        if (item.ModuleIndex == 0) item.Value = (inputStatus & (1u << 0)) != 0;
                        if (item.ModuleIndex == 1) item.Value = (inputStatus & (1u << 1)) != 0;
                        if (item.ModuleIndex == 2) item.Value = (inputStatus & (1u << 2)) != 0;
                        if (item.ModuleIndex == 3) item.Value = (inputStatus & (1u << 3)) != 0;
                        if (item.ModuleIndex == 4) item.Value = (inputStatus & (1u << 4)) != 0;
                        if (item.ModuleIndex == 5) item.Value = (inputStatus & (1u << 5)) != 0;
                    }
                    else if (item.IOType == IOType.OUTPut)
                    {
                        if (item.ModuleIndex == 0) item.Value = (outputStatus & (1u << 0)) != 0;
                        if (item.ModuleIndex == 1) item.Value = (outputStatus & (1u << 1)) != 0;
                        if (item.ModuleIndex == 2) item.Value = (outputStatus & (1u << 2)) != 0;
                        if (item.ModuleIndex == 3) item.Value = (outputStatus & (1u << 3)) != 0;
                        if (item.ModuleIndex == 4) item.Value = (outputStatus & (1u << 4)) != 0;
                        if (item.ModuleIndex == 5) item.Value = (outputStatus & (1u << 5)) != 0;
                    }
                }
            }

            return new Motion_IOStatus(inputStatus, outputStatus);
        }

        /// <summary>
        /// Updates all axis positions and velocities.
        /// </summary>
        public override void UpdateAllPosition()
        {
            if (!_isInitialized) return;

            foreach (AxtAxisData motionData in _axisData.Values)
            {
                //GetCmdPosition(motionData.AxisNo),
                motionData.Position = GetPosition(motionData.AxisNo);
                motionData.Velocity = GetVelocity(motionData.AxisNo);
            }                
        }

        /// <summary>
        /// Applies motion parameters (pulse/output mode, encoder mode, speed limits, etc.) via CAxtCAMCFS20.
        /// </summary>
        public override bool SetParameter(IAxisData motion)
        {
            var motionData = (AxtAxisData)motion;

            if (!CheckDic(motionData.AxisNo)) return false;

            SetPulseOutputMethod(motionData);
            SetEncoderInputMethod(motionData);
            SetLevels(motionData);
            SetUnitPerPulseWithGear(motionData);
            SetSpeedLimits(motionData);

            return true;
        }

        /// <summary>
        /// Retrieves current motion parameters from CAxtCAMCFS20 and updates the object.
        /// </summary>
        public override bool GetParameter(IAxisData motion)
        {
            var motionData = (AxtAxisData)motion;

            if (!CheckDic(motionData.AxisNo)) return false;

            GetPulseOutputMethod(motionData);
            GetEncoderInputMethod(motionData);
            GetLevels(motionData);
            GetUnitPerPulseWithGear(motionData);
            GetSpeedLimits(motionData);

            return true;
        }

        /// <summary>
        /// Sets signal level thresholds (limits, home, alarm) via CAxtCAMCFS20 API.
        /// </summary>
        public void SetLevels(AxtAxisData motionData)
        {
            CAxtCAMCFS20.CFS20set_pend_limit_level(motionData.AxisNo, (byte)(motionData.LvSet_EndLimitP ? 1 : 0));
            CAxtCAMCFS20.CFS20set_nend_limit_level(motionData.AxisNo, (byte)(motionData.LvSet_EndLimitN ? 1 : 0));
            CAxtCAMCFS20.CFS20set_pslow_limit_level(motionData.AxisNo, (byte)(motionData.LvSet_SlowLimitP ? 1 : 0));
            CAxtCAMCFS20.CFS20set_nslow_limit_level(motionData.AxisNo, (byte)(motionData.LvSet_SlowLimitN ? 1 : 0));
            CAxtCAMCFS20.CFS20set_inposition_level(motionData.AxisNo, (byte)(motionData.LvSet_InPosition ? 1 : 0));
            CAxtCAMCFS20.CFS20set_alarm_level(motionData.AxisNo, (byte)(motionData.LvSet_Alarm ? 1 : 0));
        }

        /// <summary>
        /// Reads signal level thresholds from CAxtCAMCFS20, logs them,
        /// and updates the <see cref="AxtAxisData"/> flags accordingly.
        /// </summary>
        public bool GetLevels(AxtAxisData motionData)
        {
            byte pendLimitLevel = CAxtCAMCFS20.CFS20get_pend_limit_level(motionData.AxisNo);
            byte nendLimitLevel = CAxtCAMCFS20.CFS20get_nend_limit_level(motionData.AxisNo);
            byte pslowLimitLevel = CAxtCAMCFS20.CFS20get_pslow_limit_level(motionData.AxisNo);
            byte nslowLimitLevel = CAxtCAMCFS20.CFS20get_nslow_limit_level(motionData.AxisNo);
            byte inpositionLevel = CAxtCAMCFS20.CFS20get_inposition_level(motionData.AxisNo);
            byte alarmLevel = CAxtCAMCFS20.CFS20get_alarm_level(motionData.AxisNo);

            Console.WriteLine($"End Limit+: {pendLimitLevel}, End Limit-: {nendLimitLevel}");
            Console.WriteLine($"Slow Limit+: {pslowLimitLevel}, Slow Limit-: {nslowLimitLevel}");
            Console.WriteLine($"Inposition: {inpositionLevel}, Alarm: {alarmLevel}");

            motionData.LvSet_EndLimitP = pendLimitLevel == 1;
            motionData.LvSet_EndLimitN = nendLimitLevel == 1;
            motionData.LvSet_SlowLimitP = pslowLimitLevel == 1;
            motionData.LvSet_SlowLimitN = nslowLimitLevel == 1;
            motionData.LvSet_InPosition = inpositionLevel == 1;
            motionData.LvSet_Alarm = alarmLevel == 1;

            return true;
        }

        /// <summary>
        /// Configures the encoder input mode for the specified axis via the CAxtCAMCFS20 API.
        /// Validates that the mode is within the valid range (0x0–0x3) before applying.
        /// </summary>
        /// <param name="motionData">The axis data to configure.</param>
        /// <returns>0 on success; negative or API error code on failure.</returns>
        public int SetEncoderInputMethod(AxtAxisData motionData)
        {
            byte method = motionData.EncInputMode;
            if (method < 0x0 || method > 0x3) return -1;
            int result = CAxtCAMCFS20.CFS20set_enc_input_method(motionData.AxisNo, method);
            return result == 0 ? 0 : result;
        }

        /// <summary>
        /// Retrieves the encoder input mode from the CAxtCAMCFS20 API and updates the axis data.
        /// </summary>
        /// <param name="motionData">The axis data to update.</param>
        /// <returns>The encoder input mode (0x0–0x3), or -1 if out of range.</returns>
        public int GetEncoderInputMethod(AxtAxisData motionData)
        {
            byte method = CAxtCAMCFS20.CFS20get_enc_input_method(motionData.AxisNo);
            if (method < 0x0 || method > 0x3) return -1;
            motionData.EncInputMode = method;
            return method;
        }

        /// <summary>
        /// Configures the pulse output mode for the specified axis via the CAxtCAMCFS20 API.
        /// Validates that the mode is within the valid range (0x0–0x7) before applying.
        /// </summary>
        /// <param name="motionData">The axis data to configure.</param>
        /// <returns>0 on success; negative or API error code on failure.</returns>
        public int SetPulseOutputMethod(AxtAxisData motionData)
        {
            byte method = motionData.PulseOutputMode;
            if (method < 0x0 || method > 0x7) return -1;
            int result = CAxtCAMCFS20.CFS20set_pulse_out_method(motionData.AxisNo, method);
            return result == 0 ? 0 : result;
        }

        /// <summary>
        /// Retrieves the pulse output mode from the CAxtCAMCFS20 API and updates the axis data.
        /// </summary>
        /// <param name="motionData">The axis data to update.</param>
        /// <returns>The pulse output mode (0x0–0x7), or -1 if out of range.</returns>
        public int GetPulseOutputMethod(AxtAxisData motionData)
        {
            byte method = CAxtCAMCFS20.CFS20get_pulse_out_method(motionData.AxisNo);
            if (method < 0x0 || method > 0x7) return -1;
            motionData.PulseOutputMode = method;
            return method;
        }

        /// <summary>
        /// Sets the start and maximum speeds for the specified axis via the CAxtCAMCFS20 API.
        /// </summary>
        /// <param name="motionData">The axis data containing MinSpeed and MaxSpeed.</param>
        public void SetSpeedLimits(AxtAxisData motionData)
        {
            CAxtCAMCFS20.CFS20set_startstop_speed(motionData.AxisNo, motionData.MinSpeed);
            CAxtCAMCFS20.CFS20set_max_speed(motionData.AxisNo, motionData.MaxSpeed);
            Console.WriteLine($"속도 설정: 시작 속도 {motionData.MinSpeed}, 최대 속도 {motionData.MaxSpeed}");
        }

        /// <summary>
        /// Retrieves the start and maximum speeds from the CAxtCAMCFS20 API and updates the axis data.
        /// </summary>
        /// <param name="motionData">The axis data to update.</param>
        public void GetSpeedLimits(AxtAxisData motionData)
        {
            double minSpeed = CAxtCAMCFS20.CFS20get_startstop_speed(motionData.AxisNo);
            double maxSpeed = CAxtCAMCFS20.CFS20get_max_speed(motionData.AxisNo);
            Console.WriteLine($"[Axis {motionData.AxisNo}] 현재 속도 제한: MinSpeed={minSpeed} Unit/sec, MaxSpeed={maxSpeed} Unit/sec");
            motionData.MinSpeed = minSpeed;
            motionData.MaxSpeed = maxSpeed;
        }

        /// <summary>
        /// Calculates the distance per pulse based on gear ratio and pulses per revolution,
        /// then configures it via the CAxtCAMCFS20 API.
        /// </summary>
        /// <param name="motionData">The axis data to configure.</param>
        public void SetUnitPerPulseWithGear(AxtAxisData motionData)
        {
            if (motionData.GearRatio <= 0 || motionData.PulsesPerRev <= 0)
            {
                return;
                throw new ArgumentException("GearRatio와 PulsesPerRev 값은 0보다 커야 합니다.");
            }

            motionData.MoveUnitPerPulse = motionData.LeadPitch / (motionData.PulsesPerRev * (1.0 / motionData.GearRatio));
            CAxtCAMCFS20.CFS20set_moveunit_perpulse(motionData.AxisNo, motionData.MoveUnitPerPulse);
            Console.WriteLine($"[Axis {motionData.AxisNo}] UnitPerPulse 설정됨: {motionData.MoveUnitPerPulse} mm/pulse");
        }

        /// <summary>
        /// Retrieves the current distance-per-pulse setting from the CAxtCAMCFS20 API,
        /// updates the axis data, and returns the value.
        /// </summary>
        /// <param name="motionData">The axis data to update.</param>
        /// <returns>The current UnitPerPulse (mm/pulse).</returns>
        public double GetUnitPerPulseWithGear(AxtAxisData motionData)
        {
            double unitPerPulse = CAxtCAMCFS20.CFS20get_moveunit_perpulse(motionData.AxisNo);
            motionData.MoveUnitPerPulse = unitPerPulse;
            Console.WriteLine($"[Axis {motionData.AxisNo}] 현재 UnitPerPulse 값: {unitPerPulse} mm/pulse");
            return unitPerPulse;
        }

        /// <summary>
        /// Opens the device, reads axis and module information,
        /// applies default settings, and populates universal I/O data.
        /// </summary>
        public void OpenDevice()
        {
            // 축 개수 가져오기
            int axisCount = CAxtCAMCFS20.CFS20get_total_numof_axis();
            int outputCounter = _universalIOData.Values.Count(x => x.IOType == IOType.OUTPut);
            int inputCounter = _universalIOData.Values.Count(x => x.IOType == IOType.InPut);

            for (int i = 0; i < axisCount; i++)
            {
                short boardNo = 0, moduleNo = 0, axisPos = 0;
                byte moduleID = 0;

                // 축 정보를 가져옴
                CAxtCAMCFS20.CFS20get_axis_info((short)i, ref boardNo, ref moduleNo, ref moduleID, ref axisPos);

                // 모듈 ID에 따라 이름 결정
                string moduleName = moduleID switch
                {
                    0x01 => "CAMC-5M, 1 Axis",
                    0x02 => "CAMC-FS, 2 Axis",
                    0x33 => "MCX314, 4 Axis",
                    _ => $"Unknown Module (ID: {moduleID:X2})"
                };

                // motionDataDictionary에 축 정보 추가
                _axisData.Add(i, new AxtAxisData
                {
                    Controller = this,
                    AxisName = $"MT{i:00}",
                    StrAxisData = "",
                    AxisNo = (short)i,
                    BoardNo = boardNo,
                    ModuleID = moduleID,
                    ModuleName = moduleName
                });

                //기본 설정 적용
                DefaultSet(i);

                // 각 축에 대해 6개의 입력 I/O 데이터를 추가
                for (int j = 0; j < 6; j++)
                {
                    _universalIOData.Add($"MX{inputCounter + _iocount["MDInput"]:X3}", new DIOData
                    {
                        Controller = this,
                        WireName = $"MX{inputCounter + _iocount["MDInput"]:X3}",
                        IOType = IOType.InPut,
                        ControllerType = ControllerType.Motion_AjinAXT,
                        ModuleName = moduleName,
                        AxisNo = (short)i,
                        ModuleIndex = j,
                    });
                    inputCounter++;
                }

                // 각 축에 대해 6개의 출력 I/O 데이터를 추가
                for (int j = 0; j < 6; j++)
                {
                    _universalIOData.Add($"MY{outputCounter + _iocount["MDOutput"]:X3}", new DIOData
                    {
                        Controller = this,
                        WireName = $"MY{outputCounter + _iocount["MDOutput"]:X3}",
                        IOType = IOType.OUTPut,
                        ControllerType = ControllerType.Motion_AjinAXT,
                        ModuleName = moduleName,
                        AxisNo = (short)i,
                        ModuleIndex = j,
                    });
                    outputCounter++;
                }
            }

            _iocount["MDInput"] = _iocount["MDInput"] + inputCounter;
            _iocount["MDOutput"] = _iocount["MDOutput"] + outputCounter;
        }

        private void DefaultSet(int i)
        {
            //0~3번 모터드라이브가 파나소닉은 정상 나머지는 반전시켜야 한다
            _axisData[i].ServoEnabledReversal = (_axisData[i].AxisNo >= 0 && _axisData[i].AxisNo <= 3) ? false : true;

            //리미트 센서 A접있음.
            bool _EndLimitP = (_axisData[i].AxisNo >= 9 && _axisData[i].AxisNo <= 15) ? true : false;

            //센서 레벨 셋팅 센서가 A접인지 B접인지
            _axisData[i].LvSet_EndLimitP = _EndLimitP;
            _axisData[i].LvSet_EndLimitN = false;
            _axisData[i].LvSet_SlowLimitP = true;
            _axisData[i].LvSet_SlowLimitN = true;
            _axisData[i].LvSet_InPosition = false;
            _axisData[i].LvSet_Alarm = false;

            _axisData[i].EncInputMode = 0x3;
            _axisData[i].PulseOutputMode = 0x4;

            //펄스당 이동거리 셋팅
            //data.LeadPitch = 25.4; //로딩 x
            _axisData[i].LeadPitch = 10;
            _axisData[i].GearRatio = 1.0;
            _axisData[i].PulsesPerRev = 10000;

            //속도 리미트 설정
            _axisData[i].MinSpeed = 1.0;
            _axisData[i].MaxSpeed = 100;

            SetParameter(_axisData[i]);
        }

        /// <summary>
        /// Returns the dictionary of motion data as IAxisData instances.
        /// </summary>
        public override Dictionary<int, IAxisData> GetMotionDataDictionary()
        {
            return _axisData.ToDictionary(
                kvp => kvp.Key, kvp => (IAxisData)kvp.Value
            );
        }

        /// <summary>
        /// Performs cleanup on application exit:
        /// stops background threads, powers off servos, and saves parameters.
        /// </summary>
        public override void MotionCtrlDispose()
        {
            Console.WriteLine("AxtMotionController 종료 중...");

            // 🛑 백그라운드 스레드 정리 (먼저 종료)
            //_threadManager.Dispose();

            // 🛑 모든 서보 OFF
            foreach (var axis in _axisData.Keys)
            {
                try
                {
                    SetServoOnOff(axis, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"축 {axis} 서보 오프 실패: {ex.Message}");
                }
            }

            // 🔄 파라미터 저장
            //axtmtclass.SaveParameters();

            Console.WriteLine("AxtMotionController 종료 완료.");
        }

        /// <summary>
        /// Moves the specified axis to a relative position with given velocity and acceleration.
        /// </summary>
        public override void MoveToPosition(int axis, double position, double velocity, double acceleration)
        {
            if (!CheckDic(axis)) return;

            _axisData[axis]._stopRequested = true;
            _axisData[axis].CurrentPosition = position;
            _axisData[axis].CurrentVelocity = velocity;
            _axisData[axis].CurrentAcceleration = acceleration;
            Move(_axisData[axis]);
        }

        /// <summary>
        /// Moves the specified axis to an absolute position with given velocity and acceleration.
        /// </summary>
        public override void MoveToPoint(int axis, double position, double velocity, double acceleration)
        {
            if (!CheckDic(axis)) return;

            //접시물 3Cm 준비!@!
            double currPos = GetPosition(axis);
            _axisData[axis]._stopRequested = true;
            _axisData[axis].CurrentPosition = position - currPos;
            _axisData[axis].CurrentVelocity = velocity;
            _axisData[axis].CurrentAcceleration = acceleration;
            Move(_axisData[axis]);
        }

        /// <summary>
        /// Executes a relative move command via the CAxtCAMCFS20 API.
        /// </summary>
        /// <param name="motionData">The axis data specifying target, velocity, acceleration.</param>
        public void Move(AxtAxisData motionData)
        {
            var result = CAxtCAMCFS20.CFS20start_r_move(motionData.AxisNo, motionData.CurrentPosition, motionData.CurrentVelocity, motionData.CurrentAcceleration);
            if (result != 0)
            {
                // 실제 AXT 결과 코드와 매칭되는 에러 메시지로 바꿔도 좋음
#if DEBUG
                Debug.WriteLine($"[AXT] 축 {motionData.AxisNo} 이동 실패: Target={motionData.CurrentPosition}, Result={result}");
#endif
            }
            else
            {
#if DEBUG
                Debug.WriteLine($"[AXT] 축 {motionData.AxisNo} 위치 {motionData.CurrentPosition} 이동 명령 성공");
#endif
            }
        }

        /// <summary>
        /// Performs repeated moves according to the provided positions and count, asynchronously.
        /// </summary>
        /// <param name="motionData">The axis data to move.</param>
        /// <param name="position">Array of target positions.</param>
        /// <param name="repeatCount">Number of repetitions.</param>
        public async Task Repeat(AxtAxisData motionData, double[] position, int repeatCount)
        {
            try
            {
                await Task.Run(() => ExecuteRepeat(motionData, position, repeatCount));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"반복 실행 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes the repeat operation internally.
        /// Performs moves for each step in each repetition, checks status after each move,
        /// and exits immediately if a stop request is detected.
        /// </summary>
        /// <param name="motionData">The MotionData object for which to perform repeated moves.</param>
        /// <param name="position">Array of target positions for each step.</param>
        /// <param name="repeatCount">Number of repetitions.</param>
        private void ExecuteRepeat(AxtAxisData motionData, double[] position, int repeatCount)
        {
            for (int repeat = 0; repeat < repeatCount; repeat++)
            {
                Console.WriteLine($"반복 {repeat + 1}/{repeatCount} 시작");
                for (int step = 0; step < position.Length; step++)
                {
                    if (motionData._stopRequested) return;
                    motionData.CurrentPosition = position[step] - motionData.Position;
                    Move(motionData);
                    while (IsHomeSearchInProgress(motionData.AxisNo))
                    {
                        if (motionData._stopRequested)
                        {
                            Stop(motionData);
                            return;
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                    System.Threading.Thread.Sleep(2000);
                }
                Console.WriteLine($"반복 {repeat + 1}/{repeatCount} 완료");
            }
            motionData._stopRequested = true;
        }

        /// <summary>
        /// Checks via the CAxtCAMCFS20 API whether a home search is still in progress.
        /// </summary>
        /// <param name="axisNo">Axis number to check.</param>
        /// <returns>True if home search is in progress; otherwise false.</returns>
        private bool IsHomeSearchInProgress(short axisNo)
        {
            return CAxtCAMCFS20.CFS20in_motion(axisNo) == 1;
        }

        /// <summary>
        /// Starts the repeat operation for the given axis, position array, velocity, acceleration, and count.
        /// </summary>
        public override void Repeat(int axis, double[] position, double velocity, double acceleration, int repeatCount)
        {
            if (!CheckDic(axis)) return;

            //_ajinMotionDataDic[axis].Position = position - _ajinMotionDataDic[axis].CurrentStatus.CurrentActPos;
            _axisData[axis].CurrentVelocity = velocity;
            _axisData[axis].CurrentAcceleration = acceleration;
            //axtmtclass.MoveToPosition(_ajinMotionDataDic[axis]);
            _axisData[axis]._stopRequested = false;
            _ = Repeat(_axisData[axis], position, repeatCount);
        }

        /// <summary>
        /// Stops motion for the given MotionData object via the CAxtCAMCFS20 API.
        /// If no stop request is pending, also halts home search.
        /// </summary>
        /// <param name="motionData">The MotionData object to stop.</param>
        public void Stop(AxtAxisData motionData)
        {
            CAxtCAMCFS20.CFS20set_stop(motionData.AxisNo);
            Console.WriteLine($"축 {motionData.AxisNo} 정지 명령이 실행되었습니다.");
            if (motionData._stopRequested == false)
            {
                StopHomeSearch(motionData);
            }
        }

        /// <summary>
        /// Issues an emergency stop via the CAxtCAMCFS20 API,
        /// then halts home search if no stop request is pending.
        /// </summary>
        /// <param name="motionData">The MotionData object to emergency stop.</param>
        public void EmergencyStop(AxtAxisData motionData)
        {
            CAxtCAMCFS20.CFS20set_e_stop(motionData.AxisNo);
            Console.WriteLine("급정지 명령이 실행되었습니다.");
            if (motionData._stopRequested == false)
            {
                StopHomeSearch(motionData);
            }
        }

        /// <summary>
        /// Cancels an ongoing home search.
        /// Sets the home state to false, raises the stop request flag,
        /// and logs the cancellation.
        /// </summary>
        /// <param name="motionData">The MotionData object whose home search to stop.</param>
        public void StopHomeSearch(AxtAxisData motionData)
        {
            motionData.HomeState = false;
            motionData._stopRequested = true;
            Console.WriteLine("원점 검색 중지 요청됨...");
        }

        /// <summary>
        /// Stops motion on the specified axis.
        /// </summary>
        /// <param name="axis">Axis number to stop.</param>
        /// <returns>True if successfully stopped; otherwise false.</returns>
        public override bool StopMotion(int axis)
        {
            if (!CheckDic(axis)) return false;

            try
            {
                Stop(_axisData[axis]);
                return true; // 성공적으로 모션 정지됨
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StopMotion] 축 {axis} 모션 정지 실패: {ex.Message}");
                return false; // 실패 시 false 반환
            }
        }

        /// <summary>
        /// Gets the current actual position of the specified axis.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <returns>Actual position, or 0 if axis is invalid.</returns>
        public override double GetPosition(int axis)
        {
            if (!CheckDic(axis)) return 0;

            _axisData[axis].Position = CAxtCAMCFS20.CFS20get_actual_position((short)axis);

            return _axisData[axis].Position;
        }

        /// <summary>
        /// Gets the commanded position of the specified axis.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <returns>Commanded position, or 0 if axis is invalid.</returns>
        public override double GetCmdPosition(int axis)
        {
            if (!CheckDic(axis)) return 0;

            _axisData[axis].CurrentPosition = CAxtCAMCFS20.CFS20get_command_position((short)axis);

            return _axisData[axis].CurrentPosition;
        }

        /// <summary>
        /// Retrieves the current velocity of the specified axis via the CAxtCAMCFS20 API.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <returns>Velocity, or 0 if axis is invalid.</returns>
        public override double GetVelocity(int axis)
        {
            if (!CheckDic(axis)) return 0;

            _axisData[axis].Velocity = CAxtCAMCFS20.CFS20get_velocity((short)axis);

            return _axisData[axis].Velocity;
        }

        /// <summary>
        /// Enables or disables the servo on the specified axis,
        /// applying reversal logic if configured.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <param name="enabled">Desired servo state.</param>
        public override void SetServoOnOff(int axis, bool enabled)
        {
            if (!CheckDic(axis)) return;

            bool _enable = enabled ^ _axisData[axis].ServoEnabledReversal; // XOR 연산을 사용한 반전 처리
            int result = CAxtCAMCFS20.CFS20set_servo_enable(_axisData[axis].AxisNo, (byte)(_enable ? 1 : 0));
            if (result == 1)
            {
                _axisData[axis].ServoEnabled = _enable;
                Console.WriteLine($"[DEBUG] 축 {_axisData[axis].AxisNo} 서보 상태 변경: {(enabled ? "On" : "Off")}");
            }
            else
            {
                Console.WriteLine("서보 상태 변경 실패");
            }
        }

        /// <summary>
        /// Checks the current servo state of the specified axis via the CAxtCAMCFS20 API,
        /// applying reversal logic if configured.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <returns>True if servo is on; otherwise false.</returns>
        public override bool IsServo(int axis)
        {
            if (!CheckDic(axis)) 
                return false;

            //return _axisData[axis].ServoEnabled;

            var axisData = _axisData[axis];

            // AXT 라이브러리에서 실제 서보 상태 가져오기
            int result = CAxtCAMCFS20.CFS20get_servo_enable(axisData.AxisNo);

            // ServoEnabledReversal을 반영한 실제 상태 계산
            bool rawState = (result == 1);
            axisData.ServoEnabled = axisData.ServoEnabledReversal ? !rawState : rawState;

            return axisData.ServoEnabled;
        }

        /// <summary>
        /// Clears any active alarm on the specified axis asynchronously.
        /// </summary>
        /// <param name="axis">Axis number.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public override Task ClearAlarm(int axis)
        {
            if (!CheckDic(axis)) return Task.CompletedTask;

            return ClearAlarm(_axisData[axis]) ?? Task.CompletedTask;
        }

        /// <summary>
        /// Clears alarm for the given MotionData asynchronously:
        /// sets the output bit, waits, then resets it.
        /// </summary>
        /// <param name="motionData">The MotionData object whose alarm to clear.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ClearAlarm(AxtAxisData motionData)
        {
            try
            {
                return ExecuteClearAlarm(motionData); // OK
            }
            catch (Exception ex)
            {
                Console.WriteLine($"홈 검색 중 오류 발생: {ex.Message}");
                return Task.CompletedTask; // ✅ 빈 Task 반환 (null 아님)
            }
        }

        /// <summary>
        /// Internal helper that performs the actual asynchronous alarm-clear sequence.
        /// </summary>
        /// <param name="motionData">The MotionData object.</param>
        private async Task ExecuteClearAlarm(AxtAxisData motionData)
        {
            int result = CAxtCAMCFS20.CFS20set_output_bit(motionData.AxisNo, 1);
            if (result != 1)
            {
                Console.WriteLine("알람 클리어 신호 설정 실패");
            }

            await Task.Delay(1000); // ✅ async 방식으로 대기

            result = CAxtCAMCFS20.CFS20reset_output_bit(motionData.AxisNo, 1);
            if (result != 1)
            {
                Console.WriteLine("알람 클리어 신호 리셋 실패");
            }
            else
            {
                Console.WriteLine("알람 클리어 성공");
            }
        }

        /// <summary>
        /// Initiates a home move on the specified axis with the given home configuration.
        /// </summary>
        public async override Task HomeMove(int axis, Motion_HomeConfig initset)
        {
            if (!CheckDic(axis)) return;

            //double[] velocities;
            //double[] accelerations;
            //double[] offsets;
            //bool[] Direction;
            //byte[] DETECT_SIGNAL;

            //// 원점 검색 설정 값
            //velocities = new double[] { 10.0, 1.0 };     // 1스텝: 빠르게, 2스텝: 느리게
            //accelerations = new double[] { 5, 0.1 };   // 1스텝: 높은 가속도, 2스텝: 낮은 가속도
            //offsets = new double[] { 0.0, 0.0 };             // 오프셋 값 설정 (필요할 경우 조정)
            //Direction = new bool[] { false, true };             // 방향
            //DETECT_SIGNAL = new byte[]
            //{
            //    (byte)DETECT_DESTINATION_SIGNAL.NElmPositiveEdge,  // 1스텝
            //    (byte)DETECT_DESTINATION_SIGNAL.NElmNegativeEdge,   // 2스텝        
            //};

            _axisData[axis].Motion_HomeCfg = initset;//new Motion_HomeConfig(velocities, accelerations, offsets, Direction, DETECT_SIGNAL);
            _axisData[axis]._stopRequested = false;
            _ = StartHomeSearch(_axisData[axis], _axisData[axis].Motion_HomeCfg);

            await Task.Delay(1);
        }

        /// <summary>
        /// Starts the asynchronous home search process using provided parameters.
        /// </summary>
        /// <param name="motionData">The MotionData object.</param>
        /// <param name="searchParams">Home search configuration.</param>
        public async Task StartHomeSearch(AxtAxisData motionData, Motion_HomeConfig searchParams)
        {
            try
            {
                await Task.Run(() => ExecuteHomeSearchSteps(motionData, searchParams));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"홈 검색 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// Executes each step of the home search sequence.
        /// On completion, resets positions to zero and signals home success.
        /// </summary>
        /// <param name="motionData">The MotionData object.</param>
        /// <param name="searchParams">Home search configuration.</param>
        private async Task ExecuteHomeSearchSteps(AxtAxisData motionData, Motion_HomeConfig searchParams)
        {

            motionData.HomeState = false;
            for (int step = 0; step < searchParams.HomeVelocities.Length; step++)
            {
                int result = PerformHomeSearchStep(motionData, step);

                if (motionData._stopRequested)
                {
                    motionData._stopRequested = true;
                    motionData.HomeState = false;
                    return;
                }

                if (result != 1)
                {
                    FinalizeHomeSuccess(motionData);
                    return;
                }

                while (IsHomeSearchInProgress(motionData.AxisNo))
                {
                    if (motionData._stopRequested)
                    {
                        Stop(motionData);
                        FinalizeHomeSuccess(motionData);
                        return;
                    }
                    await Task.Delay(100);
                    //Thread.Sleep(100);
                }
                await Task.Delay(100);
                //Thread.Sleep(100);
                Console.WriteLine("원점 비정상 종료!");
                motionData.HomeState = false;
            }

            FinalizeHomeSuccess(motionData);
        }

        /// <summary>
        /// Performs one step of the home search sequence using the CAxtCAMCFS20 API.
        /// </summary>
        /// <param name="motionData">The MotionData object.</param>
        /// <param name="step">Index of the current home search step.</param>
        /// <returns>1 on success; error code otherwise.</returns>
        private int PerformHomeSearchStep(AxtAxisData motionData, int step)
        {
            Motion_HomeConfig searchParams = motionData.Motion_HomeCfg;
            if (motionData._stopRequested) return -1;
            double searchVelocity = searchParams.HomeDirection[step] ? searchParams.HomeVelocities[step] : -searchParams.HomeVelocities[step];
            int result = CAxtCAMCFS20.CFS20start_signal_search2(motionData.AxisNo, searchVelocity, searchParams.HomeDetectSignal[step]);
            if (result == 1)
            {
                Console.WriteLine($"축 {motionData.AxisNo} {step + 1}단계 홈 검색 시작. 속도: {searchVelocity}, 가속도: {searchParams.HomeAccelerations[step]}, 신호: {searchParams.HomeDetectSignal[step]}");
            }
            else
            {
                Console.WriteLine($"축 {motionData.AxisNo} {step + 1}단계 홈 검색 실패: 오류 코드 {result}");
            }
            return result;
        }

        private void FinalizeHomeSuccess(AxtAxisData motionData)
        {
            CAxtCAMCFS20.CFS20set_command_position(motionData.AxisNo, 0);
            CAxtCAMCFS20.CFS20set_actual_position(motionData.AxisNo, 0);
            Console.WriteLine("원점 검색 완료!");
            motionData._homeSearchCompletedEvent.Set();
            motionData._stopRequested = true;
            motionData.HomeState = true;
        }

        /// <summary>
        /// Returns whether the specified axis has completed a home move.
        /// </summary>
        public override bool IsHomed(int axis)
        {
            if (!CheckDic(axis)) return false;

            return _axisData[axis].HomeState;
        }

        /// <summary>
        /// Returns whether the specified axis currently has an alarm active.
        /// </summary>
        public override bool IsAlarm(int axis)
        {
            if (!CheckDic(axis)) return false;

            short axisNo = _axisData[axis].AxisNo;
            byte alarmSwitch = CAxtCAMCFS20.CFS20get_alarm_switch(axisNo);

            _axisData[axis].Alarm = alarmSwitch == 1;

            return _axisData[axis].Alarm;
        }

        /// <summary>
        /// Returns whether the specified axis is currently in position (not moving).
        /// </summary>
        public override bool IsMoving(int axis)
        {
            if (!CheckDic(axis)) return false;

            short axisNo = _axisData[axis].AxisNo;
            byte inPosition = CAxtCAMCFS20.CFS20get_inposition_switch(axisNo);

            _axisData[axis].InPosition = inPosition == 1;

            return _axisData[axis].InPosition;
        }

        /// <summary>
        /// Returns whether the positive limit switch is active for the specified axis.
        /// </summary>
        public override bool IsPositiveLimit(int axis)
        {
            if (!CheckDic(axis)) return false;

            short axisNo = _axisData[axis].AxisNo;
            byte plimitSwitch = CAxtCAMCFS20.CFS20get_pend_limit_switch(axisNo);

            _axisData[axis].PositiveLimit = plimitSwitch == 1;

            return _axisData[axis].PositiveLimit;
        }

        /// <summary>
        /// Returns whether the negative limit switch is active for the specified axis.
        /// </summary>
        public override bool IsNegativeLimit(int axis)
        {
            if (!CheckDic(axis)) return false;

            short axisNo = _axisData[axis].AxisNo;
            byte nlimitSwitch = CAxtCAMCFS20.CFS20get_nend_limit_switch(axisNo);

            _axisData[axis].NegativeLimit = nlimitSwitch == 1;

            return _axisData[axis].NegativeLimit;
        }

        // ========== Universal I/O Implementation ==========

        /// <summary>
        /// Sets a digital output bit for the given axis and port.
        /// </summary>
        public override bool SetOutput(int axis, int port, bool state)
        {
            if (!CheckDic(axis)) return false;

            byte bitNo = (byte)(port % 6); // 하나의 축당 6개 비트 기준

            int result = state
                ? CAxtCAMCFS20.CFS20output_bit_on((short)axis, bitNo)
                : CAxtCAMCFS20.CFS20set_output_bit((short)axis, bitNo);

            if (result != 1)
            {
                Console.WriteLine($"[AXT] 출력 비트 설정 실패 - 축: {axis}, 포트: {port}, 상태: {state}");
                return false;
            }

            Console.WriteLine($"[AXT] 출력 비트 설정 성공 - 축: {axis}, 포트: {port}, 상태: {state}");
            return true;
        }

        /// <summary>
        /// Reads a digital input bit for the given axis and port.
        /// </summary>
        public override bool GetInput(int axis, int port)
        {
            if (!CheckDic(axis)) return false;

            byte bitNo = (byte)(port % 6); // 하나의 축당 6개 비트 기준

            int value = CAxtCAMCFS20.CFS20input_bit_on((short)axis, bitNo);
            if (value == 1)
                return true;
            if (value == 0)
                return false;

            Console.WriteLine($"[AXT] 입력 비트 조회 실패 - 축: {axis}, 포트: {port}");
            return false;
        }

        /// <summary>
        /// Reads a digital output bit state for the given axis and port.
        /// </summary>
        public override bool GetOutput(int axis, int port)
        {
            if (!CheckDic(axis)) return false;

            byte bitNo = (byte)(port % 6); // 하나의 축당 6개 비트 기준

            int value = CAxtCAMCFS20.CFS20output_bit_on((short)axis, bitNo);
            if (value == 1)
                return true;
            if (value == 0)
                return false;

            Console.WriteLine($"[AXT] 출력 비트 조회 실패 - 축: {axis}, 포트: {port}");
            return false;
        }

        /// <summary>
        /// Returns the dictionary of universal digital I/O data as IDigitalIOData instances.
        /// </summary>
        public Dictionary<string, IDigitalIOData> GetDigitalIODataDictionary()
        {
            return _universalIOData.ToDictionary(
                kvp => kvp.Key, kvp => (IDigitalIOData)kvp.Value
            );
        }

        /// <summary>
        /// Reads a single bit via the provided IDigitalIOData descriptor.
        /// </summary>
        public bool ReadBit(IDigitalIOData dioData)
        {
            if (dioData.IOType == IOType.InPut)
            {
                return GetInput(dioData.AxisNo, dioData.ModuleIndex);
            }
            else
            {
                return GetOutput(dioData.AxisNo, dioData.ModuleIndex);
            }
        }

        /// <summary>
        /// Writes a single bit via the provided IDigitalIOData descriptor.
        /// </summary>
        public bool WriteBit(IDigitalIOData dioData, bool value)
        {

            if (!CheckDic(dioData.AxisNo)) return false;

            //_axtmtclass.SetOutputBit(_axisData[dioData.AxisNo], dioData.IOIndex, value);

            return SetOutput(dioData.AxisNo, dioData.ModuleIndex, value);
        }

        /// <summary>
        /// Reads a 32-bit word from the specified I/O module (not implemented).
        /// </summary>
        public void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key)
        {

        }

        /// <summary>
        /// Writes a 32-bit word to the specified I/O module (not implemented).
        /// </summary>
        public void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value)
        {
            
        }

        /// <summary>
        /// Updates all I/O states if the controller is initialized.
        /// </summary>
        public void UpdateAllIOStates()
        {
            if(_isInitialized)
                UpdateAllIOStatus();
        }

        /// <summary>
        /// Performs any required cleanup for digital I/O control on shutdown.
        /// </summary>
        public void DigitalIOCtrlDispose()
        {
            
        }
    }    
}
