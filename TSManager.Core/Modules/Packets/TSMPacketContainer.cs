using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules.Packets
{
    public class PacketContainer<T> where T : ITSMPacket
    {
        public TSMPacketType Type { get; set; }
        public Version TargetVersion { get; set; }
        public string Token { get; set; }
        public T Data { get; set; }
    }
}
