using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Communication.Packet.Modbus;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    public class TestModbusRTU :ModbusRTU
    {
        public TestModbusRTU(ICommunicationConfig config)
            : base(config)
        {
        }

        public async Task Test_read()
        {
            var coils = await ReadCoilsAsync(0, 2, 1);
        }
    }
}
