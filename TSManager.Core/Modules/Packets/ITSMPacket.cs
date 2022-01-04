using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules.Packets
{
    public interface ITSMPacket
    {
        public TSMPacketType Type { get; }
    }
}
