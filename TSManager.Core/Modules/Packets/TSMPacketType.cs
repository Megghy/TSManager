using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSManager.Core.Modules.Packets
{
    public enum TSMPacketType
    {
        Connecte,
        Disconnect,
        ServerInfo,
        PlayerInfo,
        PluginsList,
        PlayerJoin,
        PlayerLeave,


    }
}
