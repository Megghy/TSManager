using PropertyChanged;
using Terraria;
using TShockAPI;

namespace TSManager.Data
{
    [AddINotifyPropertyChangedInterface]
    class ItemData
    {
        public ItemData() { }
        public ItemData(PlayerInfo info, NetItem item, int slot)
        {
            Owner = info;
            Item = TShock.Utils.GetItemById(item.NetId);
            Item.stack = item.Stack;
            Item.prefix = item.PrefixId;
            Slot = slot;
        }
        public ItemData(PlayerInfo info, Item item, int slot)
        {
            Owner = info;
            Item = TShock.Utils.GetItemById(item.type);
            Item.stack = item.stack;
            Item.prefix = item.prefix;
            Slot = slot;
        }
        public PlayerInfo Owner { get; set; }
        public int Slot = -1;
        public Item Item { get; set; } = new();

        public string Name { get { return Item != null ? Item.Name : "未知"; } set { } }

        public int ID { get { return Item != null ? Item.netID : 0; } set { var temp = new Item();
                temp.SetDefaults(value);
                temp.stack = Stack;
                temp.prefix = (byte)Prefix;
                Item = temp;
            } }

        public int Stack { get { return Item != null ? Item.stack : -1; } set { Item.stack = value; } }
        public int MaxStack { get { return Item != null ? Item.maxStack : 30; } set { } }
        public int Prefix { get { return Item != null ? Item.prefix : -1; } set { Item.prefix = (byte)value; } }
        public static implicit operator Item(ItemData i)
        {
            return i.Item;
        }
    }
}
