using System;

namespace TSManager.Core.Modules.Packet
{
    public abstract class BaseContainer<T> where T : IPacket
    {
        public PacketTypes Type { get; set; }
        public Version TargetVersion { get; set; }
        public string Token { get; set; }
        public T Data { get; set; }
    }
}
