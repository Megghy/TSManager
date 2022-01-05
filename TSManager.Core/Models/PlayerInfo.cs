using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSManager.Core.Modules.Packet;

namespace TSManager.Core.Models
{
    public class PlayerInfo : IPacket
    {
        public TPacketTypes Type => TPacketTypes.PlayerJoin | TPacketTypes.PlayerLeave;
        public string Name { get; set; }
        public string UUID { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public TItem[] Inventory { get; set; } = new TItem[260];
    }
}
