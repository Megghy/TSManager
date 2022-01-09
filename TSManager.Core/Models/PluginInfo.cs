using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Modules.Packet;

namespace TSManager.Core.Models
{
    /// <summary>
    /// 所连接服务器的所有插件信息
    /// </summary>
    public class PluginInfo : IPacket
    {
        public PacketTypes Type => PacketTypes.PluginsList;
        public string Name { get; set; }
        
    }
}
