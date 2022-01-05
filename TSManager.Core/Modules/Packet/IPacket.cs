using System;

namespace TSManager.Core.Modules.Packet
{
    public interface IPacket
    {
        public TPacketTypes Type { get; }
    }
}
