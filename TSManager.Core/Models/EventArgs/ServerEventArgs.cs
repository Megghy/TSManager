using TSManager.Core.Models;
using TSManager.Shared.TSMDatastructs;

namespace TSManager.Core.EventArgs
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
        public class OutputEventArgs : TServerEventArgs
        {
            public OutputEventArgs(ServerContainer from, ConsoleMessageInfo[] message) : base(from)
            {
                Message = message;
            }
            public ConsoleMessageInfo[] Message { get; private set; }
        }
    }
}
