using PropertyChanged;
using Terraria;
using TShockAPI;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    class ItemData
    {
        public ItemData(PlayerInfo info, NetItem item, int slot)
        {
            Owner = info;
            Item = TShock.Utils.GetItemById(item.NetId);
            Item.stack = item.Stack;
            Item.prefix = item.PrefixId;
            Slot = slot;
        }
        public PlayerInfo Owner { get; set; }
        public int Slot = -1;
        public Item Item { get; set; }

        public string Name { get { return Item != null ? Item.Name : "未知"; } set { } }

        public int ID { get { return Item != null ? Item.netID : 0; } set { } }

        public int Stack { get { return Item != null ? Item.stack : -1; } set { Item.stack = value; } }
        public int MaxStack { get { return Item != null ? Item.maxStack : 30; } set { } }
        public int Prefix { get { return Item != null ? Item.prefix : -1; } set { Item.prefix = (byte)value; } }

    }
}
