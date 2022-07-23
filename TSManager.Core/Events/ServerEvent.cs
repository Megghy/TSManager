using System;
using TSManager.Core.Models;
using TSManager.Shared.TSMDatastructs;
using static TSManager.Core.EventArgs.ServerEventArgs;

namespace TSManager.Core.Events
{
    public class ServerEvent
    {
        public static class ServerEventDelegates
        {
            public delegate void PlayerJoinEvent(PlayerJoinEventArgs args);

            public delegate void PlayerLeaveEvent(PlayerLeaveEventArgs args);

            public delegate void TextEvent(OutputEventArgs args);
        }

        public static event ServerEventDelegates.PlayerJoinEvent PlayerJoin;
        public static event ServerEventDelegates.PlayerLeaveEvent PlayerLeave;
        public static event ServerEventDelegates.TextEvent Text;
        internal static bool OnPlayerJoin(ServerContainer server, PlayerInfo info, out PlayerJoinEventArgs args)
        {
            args = new(server, info);
            try
            {
                PlayerJoin?.Invoke(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"<PlayerJoin> Hook handling failed.{Environment.NewLine}{ex}");
            }
            return args.Handled;
        }
        internal static bool OnPlayerLeave(ServerContainer server, PlayerInfo info, out PlayerLeaveEventArgs args)
        {
            args = new(server, info);
            try
            {
                PlayerLeave?.Invoke(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"<PlayerLeave> Hook handling failed.{Environment.NewLine}{ex}");
            }
            return args.Handled;
        }        
        internal static bool OnOutput(ServerContainer server, ConsoleMessageInfo[] message, out OutputEventArgs args)
        {
            args = new(server, message);
            try
            {
                Text?.Invoke(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"<Chat> Hook handling failed.{Environment.NewLine}{ex}");
            }
            return args.Handled;
        }
    }
}
