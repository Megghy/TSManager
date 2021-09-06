using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TShockAPI;
using TSManager.Data;
using TSManager.Modules;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI.Events
{
    internal class EBag : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix => "BagManage";
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            if (sender.Name == "BagManage_Refresh") { BagManager.Refresh(); return; }
            if (sender.DataContext is not ItemData item || PlayerManager.SelectedPlayerInfo == null)
            {
                Utils.Notice($"未选择物品", HandyControl.Data.InfoType.Warning);
                return;
            }
            if (PlayerManager.SelectedPlayerInfo.Data.inventory[item.Slot].NetId != item.ID)
            {
                Utils.Notice("此物品所在位置已变动, 请刷新");
                return;
            }
            switch (sender.Name)
            {
                case "BagManage_ChangeStack":
                    BagManager.ChangeStack(item);
                    break;
                case "BagManage_Del":
                    BagManager.DelItem(item);
                    break;
                default:
                    break;
            }
        }
        public override void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "Bag_Choose":
                    BagManager.ChangeBag(PlayerManager.SelectedPlayerInfo, (BagManager.BagType)sender.SelectedValue);
                    break;
                case "Bag_Prefix":
                    if (sender.DataContext is ItemData data)
                        BagManager.ChangePrefix(data, sender.SelectedIndex);
                    break;
                default:
                    break;
            }
        }
    }
}
