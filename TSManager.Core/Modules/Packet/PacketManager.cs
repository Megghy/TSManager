using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Models;

namespace TSManager.Core.Modules.Packet
{
    public static class PacketManager
    {
        public static bool RegistePacket<T>(string key) where T : IPacket
        {
            return true;
        }
        public static void SendPacket<T>(this ServerContainer server, T packet) where T : IPacket   
        {
            
        }
    }
}
