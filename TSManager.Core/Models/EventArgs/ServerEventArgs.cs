using TSManager.Core.Modules.Packet;

namespace TSManager.Core.Models.EventArgs
{
    public static class ServerEventArgs
    {
        public abstract class TServerEventArgs : TEventArgs
        {
            public TServerEventArgs(ServerContainer from)
            {
                From = from;
            }
            public ServerContainer From { get; private set; }
        }
        public class PlayerJoinEventArgs : TServerEventArgs
        {
            public PlayerJoinEventArgs(ServerContainer from, PlayerInfo info) : base(from)
            {
                Player = info;
            }
            public PlayerInfo Player { get; private set; }
        }
        public class PlayerLeaveEventArgs : PlayerJoinEventArgs
        {
            public PlayerLeaveEventArgs(ServerContainer from, PlayerInfo info) : base(from, info)
            {
            }
        }
        public class TextEventArgs : TServerEventArgs
        {
            public TextEventArgs(ServerContainer from, TextInfo[] message) : base(from)
            {
                Message = message;
            }
            public TextInfo[] Message { get; private set; }
        }
        public class SendPacketEventArgs : TServerEventArgs
        {
            public SendPacketEventArgs(ServerContainer from, IPacket packet) : base(from)
            {
                Packet = packet;
            }
            public IPacket Packet { get; private set; }
        }
        public class RecievePacketEventArgs : SendPacketEventArgs
        {
            public RecievePacketEventArgs(ServerContainer from, IPacket packet) : base(from, packet)
            {
            }
        }
    }
}
