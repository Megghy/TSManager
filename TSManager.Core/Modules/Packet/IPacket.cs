using System;

namespace TSManager.Core.Modules.Packet
{
    public interface IPacket
    {
        public PacketTypes Type { get; }
    }
}
