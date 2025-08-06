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
        /// ������ I/O �����͵��� ��⺰�� �����ϴ� ��ųʸ�.
        /// Ű�� ���̾� �̸�, ���� �ش� I/O ������ ��ü�Դϴ�.
        /// </summary>
        private Dictionary<string, IDigitalIOData> _digitalIOData = new Dictionary<string, IDigitalIOData>();

        private Dictionary<string, int> _iocount;
        public ComizoaDIO(Dictionary<string, int> count)
        {
            _iocount = count;
            // �׽�Ʈ�� �ʱ�ȭ �޼��� (������, ���� ������� �ּ� ó��)
            //IoType();
        }

        /// <summary>
        /// �׽�Ʈ �� ������ �ʱ�ȭ �޼���.
        /// 15���� ���� ������ I/O �����͸� �����Ͽ� ��ųʸ��� �߰��մϴ�.
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
                    WireName = $"Y{i + _iocount["DOutput"]:X3}",    // ���̾��ȣ (��: Y000, Y001, ...)
                    StrdataName = "",         // IO �̸� (�ʿ� �� ����)
                    ModuleName = "",          // ����� �̸� (��: SIO_DO32P)
                    ModuleIndex = i,              // ����� ID (Ư�� ��� �ĺ�)
                    Value = true,             // �ʱ� IO ����
                    PollingState = false,     // ���� �� ����
                    StateReversal = false,
                    Offset = 0,               // ��Ʈ ������
                    Edge = false,             // ���� ���� ����
                    DetectionTime = 0         // ���� �ð�
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
