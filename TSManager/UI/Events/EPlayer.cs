using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HandyControl.Controls;
using TSManager.Data;
using TSManager.Modules;
using TSManager.UI.Control;
using ComboBox = HandyControl.Controls.ComboBox;

namespace TSManager.UI.Events
{
    internal class EPlayer : GUIEvents.GUIEventBase
    {
        public override string ControlPrefix => "PlayerManage";
        private bool CheckPlrIsSelected(string senderName, out PlayerInfo plrInfo)
        {
            plrInfo = PlayerManager.SelectedPlayerInfo;
            if (plrInfo is null)
            {
                Utils.Notice("未选择玩家", HandyControl.Data.InfoType.Warning);
                return false;
            }
            if (!plrInfo.Online && senderName is not ("PlayerManage_Del" or "PlayerManage_UnBan" or "PlayerManage_Password" or "PlayerManage_Ban"))
            {
                Utils.Notice("玩家 " + plrInfo.Name + " 未在线.", HandyControl.Data.InfoType.Warning);
                return false;
            }
            return true;
        }
        public override void OnButtonClick(Button sender, RoutedEventArgs e)
        {
            if (CheckPlrIsSelected(sender.Name, out var plrInfo))
                switch (sender.Name)
                {

                    case "PlayerManage_Heal":
                        plrInfo.Heal();
                        break;
                    case "PlayerManage_Kill":
                        plrInfo.Kill();
                        break;
                    case "PlayerManage_Del":
                        Growl.Ask($"确定要删除玩家 {plrInfo} 的账号吗?", result =>
                        {
                            if (result) plrInfo.Del();
                            return true;
                        });
                        break;
                    case "PlayerManage_UnBan":
                        plrInfo.UnBan();
                        break;
                    default:
                        break;
                }
        }
        public override void OnButtonTextBoxClick(ButtonTextBox sender, RoutedEventArgs e)
        {
            if (CheckPlrIsSelected(sender.Name, out var plrInfo))
                switch (sender.Name)
                {
                    case "PlayerManage_Password":
                        if (plrInfo.ChangePassword(sender.Text)) sender.Text = "";
                        break;
                    case "PlayerManage_Kick":
                        plrInfo.Kick(sender.Text);
                        break;
                    case "PlayerManage_Ban":
                        plrInfo.AddBan(sender.Text);
                        break;
                    case "PlayerManage_UnBan":
                        plrInfo.UnBan();
                        break;
                    case "PlayerManage_Whisper":
                        plrInfo.Whisper(sender.Text);
                        break;
                    case "PlayerManage_Damage":
                        if (int.TryParse(sender.Text, out int damage))
                            plrInfo.Damage(damage);
                        else
                            Utils.Notice("无效的伤害数值", HandyControl.Data.InfoType.Warning);
                        break;
                    case "PlayerManage_Command":
                        plrInfo.Command(sender.Text);
                        break;
                    case "PlayerManage_GodMode":
                        plrInfo.GodMode();
                        break;
                    case "PlayerManage_Mute":
                        plrInfo.Mute();
                        break;
                    default:
                        break;
                }
        }
        public override void OnSwichClick(ToggleButton sender, RoutedEventArgs e)
        {
            if (CheckPlrIsSelected(sender.Name, out var plrInfo))
                switch (sender.Name)
                {
                    case "PlayerManage_GodMode":
                        plrInfo.GodMode();
                        break;
                    case "PlayerManage_Mute":
                        plrInfo.Mute();
                        break;
                    default:
                        break;
                }
        }
        public override void OnComboSelectChange(ComboBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "PlayerManage_Group":
                    PlayerManager.SelectedPlayerInfo.ChangeGroup(TSMMain.GUI.PlayerManage_Group.SelectedItem);
                    break;
                default:
                    break;
            }
        }
        public override void OnListBoxSelectChange(ListBox sender, SelectionChangedEventArgs e)
        {
            switch (sender.Name)
            {
                case "PlayerManage_List":
                    PlayerManager.ChangeDisplayInfo((PlayerInfo)sender.SelectedItem);
                    break;
                default:
                    break;
            }
        }
    }
}
