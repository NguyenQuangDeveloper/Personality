using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Communication.Packet.Modbus;

namespace VSLibrary.Communication.Packet.Protocol.Test
{
    public class TestModbusTCP : ModbusTCP
    {
        public TestModbusTCP(ICommunicationConfig config)
            : base(config)
        {
        }
    }
}
