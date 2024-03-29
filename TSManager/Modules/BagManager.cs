﻿using Ionic.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Terraria;
using Terraria.Localization;
using TShockAPI;
using TSManager.Data;

namespace TSManager.Modules
{
    public static class BagManager
    {
        internal static void Init()
        {
            Info.TextureZip = ZipFile.Read(new MemoryStream(Properties.Resources.Texture)); //加载贴图
            Dictionary<string, int> prefix = new();
            var tempjobj = JObject.Parse(Properties.Resources.Prefix);
            foreach (var jobj in tempjobj)
            {
                int prefixid = jobj.Key.Split("prefix_")[1].ToInt();
                prefix.Add(prefixid + " - " + (string)jobj.Value, prefixid);
            }
            TSMMain.GUI.Bag_Prefix.ItemsSource = prefix;

            //加载背包界面
            BagManager.CreateBox();
            //添加背包界面选项
            Dictionary<string, int> bags = new()
            {
                { "主背包", 0 },
                { "护甲及饰品", 1 },
                { "染料", 2 },
                { "猪猪罐", 3 },
                { "保险箱", 4 },
                { "守卫者熔炉", 5 },
                { "虚空", 6 },
                { "Buff栏", 8 },
                { "金币.弹药.被选中", 7 }
            };

            TSMMain.GUI.Bag_Choose.ItemsSource = bags;
        }
        public static void Refresh(bool showNotice = true)
        {
            if (TSMMain.GUI.PlayerManage_List.SelectedItem is PlayerInfo info)
            {
                ChangeItemList(GetPlayerBag(info, (BagType)TSMMain.GUI.Bag_Choose.SelectedIndex));
            }
            if (showNotice) Utils.Notice("已刷新背包", HandyControl.Data.InfoType.Success);
        }
        public static void DelItem(ItemData item)
        {
            if (item.Owner.Online)
            {
                ModifyItemOnline(new ItemData(item.Owner, new Item(), item.Slot));
            }
            else
            {
                item.Owner.Data.inventory[item.Slot] = new();
                item.Owner.Save();
            }
            Utils.Notice("已删除物品", HandyControl.Data.InfoType.Success);
            Refresh(false);
        }
        public static void ModifyItemOnline(ItemData data)
        {
            var player = data.Owner.Player;
            player.PlayerData.inventory[data.Slot] = new(data.ID, data.Slot, (byte)data.Prefix);
            var slot = data.Slot;
            var item = data.Item;
            int index;
            //Inventory slots
            if (slot < NetItem.InventorySlots)
            {
                index = slot;
                player.TPlayer.inventory[slot] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.inventory[index].Name), player.Index, slot, item.stack, item.prefix, item.netID);

            }

            //Armor & Accessory slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots)
            {
                index = slot - NetItem.InventorySlots;
                player.TPlayer.armor[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.armor[index].Name), player.Index, slot, item.stack, item.prefix, item.netID);
            }

            //Dye slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots);
                player.TPlayer.dye[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.dye[index].Name), player.Index, slot, player.TPlayer.dye[index].prefix);
            }

            //Misc Equipment slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots);
                player.TPlayer.miscEquips[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscEquips[index].Name), player.Index, slot, item.stack, item.prefix, item.netID);
            }

            //Misc Dyes slots
            else if (slot < NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots + NetItem.MiscDyeSlots)
            {
                index = slot - (NetItem.InventorySlots + NetItem.ArmorSlots + NetItem.DyeSlots + NetItem.MiscEquipSlots);
                player.TPlayer.miscDyes[index] = item;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, NetworkText.FromLiteral(player.TPlayer.miscDyes[index].Name), player.Index, slot, item.stack, item.prefix, item.netID);
            }
        }
        public static void CreateBox()
        {
            var gui = TSMMain.GUI;
            TSMMain.GUIInvoke(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    gui.PlayerBagBox.RowDefinitions.Add(new RowDefinition { Height = new GridLength(65) });
                    gui.PlayerBagBox.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
                    gui.PlayerBagBox.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
                }
                int row = 0;
                int col = 0;
                for (int i = 0; i < 50; i++)
                {
                    Label block = new()
                    {
                        Margin = new Thickness(5, row == 0 ? 0 : 5, 5, row == 0 ? 10 : 5),
                        Cursor = Cursors.Hand,
                        Background = null,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Bottom
                    };
                    if (col + 1 > 10)
                    {
                        col = 0;
                        row += 1;
                    }
                    block.SetValue(Grid.RowProperty, row);
                    block.SetValue(Grid.ColumnProperty, col);
                    col += 1;
                    block.MouseLeftButtonDown += SelectItem;
                    gui.PlayerBagBox.Children.Add(block);
                }
            });
        }
        public static void ClearAllItem()
        {
            for (int i = 0; i < 50; i++)
            {
                ((Label)TSMMain.GUI.PlayerBagBox.Children[i]).Background = null;
            }
        }
        public static void ChangeItemList(List<ItemData> list)
        {
            Task.Run(() =>
            {
               lock (TSMMain.GUI.PlayerBagBox)
               {
                   if (list == null) return;
                   for (int i = 0; i < list.Count; i++)
                   {
                       if (i >= list.Count)
                       {
                           ((Label)TSMMain.GUI.PlayerBagBox.Children[i]).Background = null;
                           continue;
                       }
                       DrawingGroup imageDrawings = new();
                       ImageDrawing background = new()
                       {
                           Rect = new Rect(0, 0, 60, 60),
                           ImageSource = Utils.GetTextureAsync("Bag.png").Result
                       };
                       imageDrawings.Children.Add(background);

                       var item = Utils.GetTextureAsync($"{list[i].ID}.png").Result;
                       if (item == null) continue;

                       //十分憨批的图片缩放
                       var width = item.Width;
                       var height = item.Height;
                       double top = 0;
                       double left = 0;
                       int size = 32;
                       int blocksize = 60;
                       if (height > size && width > size) //高宽都超过
                       {
                           if (width >= height)
                           {
                               height = size / width * height;
                               width = size;
                               top = (blocksize - height) / 2;
                               left = (blocksize - width) / 2;
                           }
                           else
                           {
                               width = size / height * width;
                               height = size;
                               top = (blocksize - height) / 2;
                               left = (blocksize - width) / 2;
                           }
                       }
                       else if (height > size && width <= size)  //贴图高度超过
                       {
                           width = size / height * width;
                           height = size;
                           top = (blocksize - height) / 2;
                           left = (blocksize - width) / 2;
                       }
                       else if (height <= size && width > size)  //贴图宽度超过
                       {
                           height = size / width * height;
                           width = size;
                           top = (blocksize - height) / 2;
                           left = (blocksize - width) / 2;
                       }
                       else
                       {
                           top = (blocksize - height) / 2;
                           left = (blocksize - width) / 2;
                       }
                       ImageDrawing smallKiwi1 = new()
                       {
                           Rect = new Rect(left, top, width, height),
                           ImageSource = item
                       }; //物品贴图位置
                       imageDrawings.Children.Add(smallKiwi1);
                       DrawingImage drawingImageSource = new(imageDrawings);
                       drawingImageSource.Freeze();
                       TSMMain.GUIInvoke(() =>
                       {
                           var l = (Label)TSMMain.GUI.PlayerBagBox.Children[i];
                           l.Content = list[i].Stack == 0 ? "" : list[i].Stack;
                           l.Background = new ImageBrush(drawingImageSource);
                           l.DataContext = list[i];
                       });
                   }
               }
           });
        }
        public enum BagType
        {
            inventory,
            equipment,
            dyes,
            piggy,
            safe,
            forge,
            Void,
            _inventoty,
            buff
        }
        public static List<ItemData> GetPlayerBag(PlayerInfo info, BagType type)
        {
            try
            {
                if (info is null || info.Data is null) return null;
                List<ItemData> list = new();
                var plr = info.Online ? info.Player.TPlayer : null;
                switch (type)
                {
                    case BagType.inventory:
                        for (int i = 0; i < 50; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.inventory[i].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.inventory[i].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.inventory[i].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType._inventoty:
                        for (int i = 50; i < 58; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = (int)(info.Online ? plr.inventory[i]?.type : info.Data.inventory[i].NetId),
                            Stack = (int)(info.Online ? plr.inventory[i]?.stack : info.Data.inventory[i].Stack),
                            Prefix = (int)(info.Online ? plr.inventory[i]?.prefix : info.Data.inventory[i].PrefixId),
                            Slot = i
                        });
                        if (info.Online) list.Add(new(info, plr.trashItem, 59));
                        return list;
                    case BagType.equipment:
                        for (int i = 59; i < 79; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.miscEquips[i - 59].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.miscEquips[i - 59].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.miscEquips[i - 59].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.dyes:
                        for (int i = 79; i < 89; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.dye[i - 79].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.dye[i - 79].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.dye[i - 79].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.piggy:
                        for (int i = 100; i < 140; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.bank.item[i - 100].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.bank.item[i - 100].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.bank.item[i - 100].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.safe:
                        for (int i = 140; i < 180; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.bank2.item[i - 140].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.bank2.item[i - 140].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.bank2.item[i - 140].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.forge:
                        for (int i = 180; i < 220; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.bank3.item[i - 180].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.bank3.item[i - 180].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.bank3.item[i - 180].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.Void:
                        for (int i = 220; i < 260; i++) list.Add(new()
                        {
                            Owner = info,
                            ID = info.Online ? plr.bank4.item[i - 220].type : info.Data.inventory[i].NetId,
                            Stack = info.Online ? plr.bank4.item[i - 220].stack : info.Data.inventory[i].Stack,
                            Prefix = info.Online ? plr.bank4.item[i - 220].prefix : info.Data.inventory[i].PrefixId,
                            Slot = i
                        });
                        return list;
                    case BagType.buff:
                        Utils.Notice("还没写! 先用用别的⑧");
                        break;
                }
                return list;
            }
            catch (Exception ex) { Utils.Notice($"发生内部错误, 请向开发者报告此问题.\n{ex}", HandyControl.Data.InfoType.Error); return null; }
        }
        public static void ChangeBag(PlayerInfo info, BagType type)
        {
            ChangeItemList(GetPlayerBag(info, type));
            TSMMain.GUI.Bag_ItemInfo.DataContext = null;
        }
        public static void SelectItem(object sender, RoutedEventArgs e)
        {
            try
            {
                Label label = sender as Label;
                var item = (ItemData)label.DataContext;
                TSMMain.GUI.Bag_ItemInfo.DataContext = item;
            }
            catch (Exception ex) { ex.ShowError(); }
        }
        public static void ChangePrefix(ItemData item, int prefix)
        {
            if (item.Prefix == prefix) return;
            else item.Prefix = prefix;
            if (TShock.Utils.GetItemById(item.Owner.Data.inventory[item.Slot].NetId).type != item.ID)
            {
                Utils.Notice("此物品所在位置已变动, 请刷新");
                return;
            }
            if (item.Owner.Online)
            {
                ModifyItemOnline(item);
            }
            else
            {
                item.Owner.Data.inventory[item.Slot] = new NetItem(item.ID, item.Stack, (byte)item.Prefix);
                item.Owner.Save();
            }
            Refresh(false);
            Utils.Notice("已修改物品前缀", HandyControl.Data.InfoType.Success);
        }
        public static void ChangeStack(ItemData item)
        {
            if (item.Owner.Online)
            {
                ModifyItemOnline(item);
            }
            else
            {
                item.Owner.Data.inventory[item.Slot] = new NetItem(item.ID, item.Stack, (byte)item.Prefix);
                item.Owner.Save();
            }
            Refresh(false);
            Utils.Notice($"已修改 {item.Item.Name} 的堆叠为 {item.Stack}", HandyControl.Data.InfoType.Success);
        }
    }
}
