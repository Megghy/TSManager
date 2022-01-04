using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Modules.Packets;

namespace TSManager.Core.Models
{
    /// <summary>
    /// 所连接服务器的所有插件信息
    /// </summary>
    public class PluginInfo : ITSMPacket
    {
        public TSMPacketType Type => TSMPacketType.PluginsList;
        public string Name { get; set; }
        
    }
}
