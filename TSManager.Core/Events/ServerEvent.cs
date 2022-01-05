using System;
using TSManager.Core.Models;
using TSManager.Core.Modules.Packet;
using static TSManager.Core.Models.EventArgs.ServerEventArgs;

namespace TSManager.Core.Events
{
    public class ServerEvent
    {
        public static class ServerEventDelegates
        {
            public delegate void PlayerJoinEvent(PlayerJoinEventArgs args);

            public delegate void PlayerLeaveEvent(PlayerLeaveEventArgs args);

            public delegate void TextEvent(TextEventArgs args);

            public delegate void SendPacketEvent(SendPacketEventArgs args);

            public delegate void RecievePacketEvent(RecievePacketEventArgs args);
        }

        public static event ServerEventDelegates.PlayerJoinEvent PlayerJoin;
        public static event ServerEventDelegates.PlayerLeaveEvent PlayerLeave;
        public static event ServerEventDelegates.TextEvent Text;
        public static event ServerEventDelegates.SendPacketEvent SendPacket;
        public static event ServerEventDelegates.RecievePacketEvent RecievePacket;
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
        internal static bool OnText(ServerContainer server, TextInfo[] message, out TextEventArgs args)
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
        internal static bool OnSendPacket(ServerContainer server, IPacket packet, out SendPacketEventArgs args)
        {
            args = new(server, packet);
            try
            {
                SendPacket?.Invoke(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"<SendPacket> Hook handling failed.{Environment.NewLine}{ex}");
            }
            return args.Handled;
        }
        internal static bool OnGetPacket(ServerContainer server, IPacket packet, bool fromClient, out RecievePacketEventArgs args)
        {
            args = new(server, packet);
            try
            {
                RecievePacket?.Invoke(args);
            }
            catch (Exception ex)
            {
                Logger.Error($"<GetPacket> Hook handling failed.{Environment.NewLine}{ex}");
            }
            return args.Handled;
        }
    }
}
