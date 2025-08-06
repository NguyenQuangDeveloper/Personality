using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSLibrary.Controller.DigitalIO
{
    internal class ComizoaDIO : DIOBase
    {
        /// <summary>
        /// 디지털 I/O 데이터들을 모듈별로 저장하는 딕셔너리.
        /// 키는 와이어 이름, 값은 해당 I/O 데이터 객체입니다.
        /// </summary>
        private Dictionary<string, IDigitalIOData> _digitalIOData = new Dictionary<string, IDigitalIOData>();

        private Dictionary<string, int> _iocount;
        public ComizoaDIO(Dictionary<string, int> count)
        {
            _iocount = count;
            // 테스트용 초기화 메서드 (디버깅용, 실제 운영에서는 주석 처리)
            //IoType();
        }

        /// <summary>
        /// 테스트 및 디버깅용 초기화 메서드.
        /// 15개의 더미 디지털 I/O 데이터를 생성하여 딕셔너리에 추가합니다.
        /// </summary>
        private void test()
        {
            for (int i = 0; i < 15; i++)
            {
                DIOData strdata = new DIOData
                {
                    Controller = this,
                    ControllerType = ControllerType.DIO_Comizoa,
                    IOType = IOType.OUTPut,
                    WireName = $"Y{i + _iocount["DOutput"]:X3}",    // 와이어번호 (예: Y000, Y001, ...)
                    StrdataName = "",         // IO 이름 (필요 시 설정)
                    ModuleName = "",          // 모듈의 이름 (예: SIO_DO32P)
                    ModuleIndex = i,              // 모듈의 ID (특정 모듈 식별)
                    Value = true,             // 초기 IO 상태
                    PollingState = false,     // 폴링 후 상태
                    StateReversal = false,
                    Offset = 0,               // 비트 오프셋
                    Edge = false,             // 엣지 감지 여부
                    DetectionTime = 0         // 감지 시간
                };

                _digitalIOData.Add(strdata.WireName, strdata);
            }

            _iocount["DOutput"] = _iocount["DOutput"] + 15;
        }

        public override Dictionary<string, IDigitalIOData> GetDigitalIODataDictionary()
        {
            return _digitalIOData.ToDictionary(
                kvp => kvp.Key, kvp => kvp.Value
            );
        }

        public override bool ReadBit(IDigitalIOData dioData)
        {
            throw new NotImplementedException();
        }

        public override void ReadDword(Dictionary<string, IDigitalIOData> dioDataDict, string key)
        {
            throw new NotImplementedException();
        }

        public override bool WriteBit(IDigitalIOData dioData, bool value)
        {
            throw new NotImplementedException();
        }

        public override void WriteDword(Dictionary<string, IDigitalIOData> dioDataDict, string key, uint value)
        {
            throw new NotImplementedException();
        }

        public override void UpdateAllIOStates()
        {
            //throw new NotImplementedException();
        }
    }
}
