using System;
using System.Windows.Forms;
using Terraria.GameContent.Creative;
using TShockAPI;
using TShockAPI.DB;
using TSManager.Data;

namespace TSManager.Modules
{
    static class PlayerManager
    {
        public static void ChangeDisplayInfo(PlayerInfo info)
        {
            TSMMain.GUI.PlayerManage_Info.DataContext = info;
            TSMMain.GUI.PlayerManage_Group.SelectedItem = info.Group;
            //修改背包显示
            if (TSMMain.GUI.Bag_Choose.SelectedIndex == 0) BagManager.ChangeBag(info, BagManager.BagType.inventory);
            else TSMMain.GUI.Bag_Choose.SelectedIndex = 0;
        }
        public static void ChangeGroup(this PlayerInfo info, object group)
        {
            var g = group as Group;
            if (group != null && info.Group != group)
            {
                TShock.UserAccounts.SetUserGroup(info.Account, g.Name);
                Utils.Notice($"成功将玩家 {info} 的用户组更改为 {g.Name}", HandyControl.Data.InfoType.Success);
            }
        }
        public static void ChangeGroup(this PlayerInfo info, Group group)
        {
            if (group != null && info.Group != group)
            {
                TShock.UserAccounts.SetUserGroup(info.Account, group.Name);
                Utils.Notice($"成功将玩家 {info} 的用户组更改为 {group.Name}", HandyControl.Data.InfoType.Success);
            }
        }
        public static bool ChangePassword(this PlayerInfo info, string password)
        {
            if (password.Length < TShock.Config.Settings.MinimumPasswordLength)
            {
                Utils.Notice($"密码长度应大于等于 {TShock.Config.Settings.MinimumPasswordLength}.", HandyControl.Data.InfoType.Error);
                return false;
            }
            if (!info.Account.VerifyPassword(password))
            {
                TShock.UserAccounts.SetUserAccountPassword(info.Account, password);
                Utils.Notice($"成功将玩家 {info} 的密码修改为 {password}.", HandyControl.Data.InfoType.Success);
                return true;
            }
            else
            {
                Utils.Notice("此密码与原密码相同..", HandyControl.Data.InfoType.Error);
                return false;
            }
        }
        public static void Kick(this PlayerInfo info, string reason)
        {
            if (info.Player.Kick(reason == "" ? "未指定原因" : reason, true, false, "TSManager", true))
            {
                Utils.Notice("成功踢出玩家 " + info, HandyControl.Data.InfoType.Success);
                TSMMain.GUI.PlayerManage_Kick.Text = "";
            }
            else
            {
                Utils.Notice("踢出玩家 " + info + " 失败.", HandyControl.Data.InfoType.Error);
            }
        }
        public static void AddBan(this PlayerInfo info, string reason)
        {
            if (info == null)
            {
                Utils.Notice("当前未选择玩家", HandyControl.Data.InfoType.Error);
                return;
            }
            bool success = false;
            if (info.Online)
            {
                if (info.Player.Ban(reason == "" ? "未指定原因" : reason, true, "TSManager"))
                {
                    TSMMain.GUI.PlayerManage_Ban.IsEnabled = false;
                    TSMMain.GUI.PlayerManage_UnBan.IsEnabled = true;
                    success = true;
                }
            }
            else
            {
                if (TShock.Bans.InsertBan($"{Identifier.UUID}{(info.Account ?? new UserAccount()).UUID}", reason, "TSManager", DateTime.UtcNow, DateTime.MaxValue) != null) success = true;
                if (TShock.Bans.InsertBan($"{Identifier.Account}{info.Name}", reason, "TSManager", DateTime.UtcNow, DateTime.MaxValue) != null) success = true;
            }
            if (success) { Utils.Notice("成功封禁玩家 " + info.Name, HandyControl.Data.InfoType.Success); TSMMain.GUI.PlayerManage_Ban.Text = ""; }
            else Utils.Notice("封禁 " + info + " 失败", HandyControl.Data.InfoType.Error);
        }
        public static void UnBan(this PlayerInfo info)
        {

        }
        public static void Whisper(this PlayerInfo info, string text)
        {
            info.Player.LastWhisper = TSPlayer.Server;
            info.Player.SendMessage($"<私聊> {text}", Microsoft.Xna.Framework.Color.Aqua);
            Utils.Notice("成功发送私聊 " + text, HandyControl.Data.InfoType.Success);
        }
        public static void Command(this PlayerInfo info, string text)
        {
            if (Commands.HandleCommand(info.Player, text.StartsWith(Commands.Specifier) ? text : "/" + text)) Utils.Notice("成功执行命令 " + text, HandyControl.Data.InfoType.Success);
            else Utils.Notice("执行失败", HandyControl.Data.InfoType.Error);
        }
        public static void Damage(this PlayerInfo info, int damage)
        {
            info.Player.DamagePlayer(damage);
            Utils.Notice("成功执行操作", HandyControl.Data.InfoType.Success);
        }
        public static void Heal(this PlayerInfo info)
        {
            info.Player.Heal(info.Player.TPlayer.statLifeMax2);
            Utils.Notice("成功治疗玩家", HandyControl.Data.InfoType.Success);
        }
        public static void GodMode(this PlayerInfo info)
        {
            info.Player.GodMode = !info.Player.GodMode;
            CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(info.Player.Index, info.Player.GodMode);
            Utils.Notice($"上帝模式已{(info.Player.GodMode ? "开启" : "关闭")}", HandyControl.Data.InfoType.Success);
            info.Player.SendInfoMessage($"你已{(info.Player.GodMode ? "开启" : "关闭")}上帝模式");
        }
        public static void Mute(this PlayerInfo info)
        {
            info.Player.mute = !info.Player.mute;
            Utils.Notice($"已{(info.Player.GodMode ? "开启" : "关闭")}禁言", HandyControl.Data.InfoType.Success);
            info.Player.SendInfoMessage($"你已被{(info.Player.mute ? "" : "取消")}禁言");
        }
        public static void Kill(this PlayerInfo info)
        {
            info.Player.KillPlayer();
            Utils.Notice("已击杀玩家", HandyControl.Data.InfoType.Success);
        }
        public static void Del(this PlayerInfo info)
        {
            if (info.Online) info.Player.SendInfoMessage("该账号已被移除");
            TShock.UserAccounts.RemoveUserAccount(info.Account);
            Info.Players.Remove(info);
            TSMMain.GUIInvoke(() => {
                TSMMain.GUI.PlayerManage_MainTab.DataContext = null;
                TSMMain.GUI.Bag_Tab.DataContext = null;
            });
            Utils.Notice($"已删除玩家 {info.Name}", HandyControl.Data.InfoType.Success);
        }
    }
}
