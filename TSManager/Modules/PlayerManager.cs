using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Terraria.GameContent.Creative;
using TShockAPI;
using TShockAPI.DB;
using TSManager.Data;

namespace TSManager.Modules
{
    public static class PlayerManager
    {
        public static PlayerInfo SelectedPlayerInfo => TSMMain.GUI.PlayerManage_List.SelectedItem as PlayerInfo;
        public static void ChangeDisplayInfo(PlayerInfo info)
        {
            TSMMain.GUI.PlayerManage_Info.DataContext = info;
            TSMMain.GUI.PlayerManage_Group.SelectedItem = info?.Group;
            //修改背包显示
            if (TSMMain.GUI.Bag_Choose.SelectedIndex == 0)
                BagManager.ChangeBag(info, BagManager.BagType.inventory);
            else
                TSMMain.GUI.Bag_Choose.SelectedIndex = 0;
        }
        public static void Refresh()
        {
            TSMMain.GUIInvoke(() =>
            {
                Info.Players = new(PlayerInfo.GetAllPlayerInfo());
                TSMMain.GUI.PlayerManage_List.ItemsSource = Info.Players;
                if (Info.Players.Any())
                    TSMMain.GUI.PlayerManage_List.SelectedItem = Info.Players[0];
            });
        }
        /// <summary>
        /// 修改玩家所处的用户组.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="group">由于部分代码写于未加载ts之前, 为避免出错使用object</param>
        public static void ChangeGroup(this PlayerInfo info, object group)
        {
            var g = group as Group;
            if (group != null && info.Group != group)
            {
                TShock.UserAccounts.SetUserGroup(info.Account, g.Name);
                Utils.Notice($"成功将玩家 {info} 的用户组更改为 {g.Name}", HandyControl.Data.InfoType.Success);
            }
        }
        public static bool ChangePassword(this PlayerInfo info, string password)
        {
            if (password.Length < TShock.Config.Settings.MinimumPasswordLength)
                Utils.Notice($"密码长度应大于等于 {TShock.Config.Settings.MinimumPasswordLength}.", HandyControl.Data.InfoType.Warning);
            else if (info.Account is null)
                Utils.Notice("无效的账号", HandyControl.Data.InfoType.Warning);
            else if (info.Account.VerifyPassword(password))
                Utils.Notice("此密码与原密码相同", HandyControl.Data.InfoType.Warning);
            else
            {
                TShock.UserAccounts.SetUserAccountPassword(info.Account, password);
                Utils.Notice($"成功将玩家 {info} 的密码修改为 {password}.", HandyControl.Data.InfoType.Success);
                return true;
            }
            return false;
        }
        #region 一些对playerinfo的拓展函数
        public static void Kick(this PlayerInfo info, string reason)
        {
            if (info.Player.Kick(string.IsNullOrWhiteSpace(reason) ? "未指定原因" : reason, true, false, "TSManager", true))
            {
                Utils.Notice("成功踢出玩家 " + info, HandyControl.Data.InfoType.Success);
                TSMMain.GUI.PlayerManage_Kick.Text = "";
            }
            else
                Utils.Notice("踢出玩家 " + info + " 失败.", HandyControl.Data.InfoType.Warning);
        }
        public static void AddBan(this PlayerInfo info, string reason)
        {
            if (info == null)
            {
                Utils.Notice("当前未选择玩家", HandyControl.Data.InfoType.Warning);
                return;
            }
            if (info.Online)
            {
                if (info.Player.Ban(string.IsNullOrWhiteSpace(reason) ? "未指定原因" : reason, true, "TSManager"))
                    Success(info);
            }
            else
            {
                if (TShock.Bans.InsertBan($"{Identifier.UUID}{(info.Account ?? new UserAccount()).UUID}", reason, "TSManager", DateTime.UtcNow, DateTime.MaxValue) != null && TShock.Bans.InsertBan($"{Identifier.Account}{info.Name}", reason, "TSManager", DateTime.UtcNow, DateTime.MaxValue) != null)
                    Success(info);
                else
                    Utils.Notice("封禁 " + info + " 失败", HandyControl.Data.InfoType.Warning);
            }
            static void Success(PlayerInfo info)
            {
                Utils.Notice("成功封禁玩家 " + info.Name, HandyControl.Data.InfoType.Success);
                TSMMain.GUI.PlayerManage_Ban.Text = "";
                TSMMain.GUI.PlayerManage_Ban.IsEnabled = false;
                TSMMain.GUI.PlayerManage_UnBan.IsEnabled = true;
            }
        }
        public static void UnBan(this PlayerInfo info)
        {
            var list = new List<Ban>();
            TShock.Bans.Bans
                .Where(b => b.Value.Identifier == "uuid:" + info.Account?.UUID)
                .ToList()
                .ForEach(b => TShock.Bans.RemoveBan(b.Key, true));
            TShock.Bans.Bans
                .Where(b => b.Value.Identifier == "acc:" + info)
                .ToList()
                .ForEach(b => TShock.Bans.RemoveBan(b.Key, true));
            if (JsonConvert.DeserializeObject<List<string>>(info.Account?.KnownIps??"[]") is { Count: > 0 } ips)
                TShock.Bans.Bans
                    .Where(b => b.Value.Identifier.StartsWith("ip") && ips.Contains(b.Value.Identifier.Replace("ip:", "")))
                    .ToList()
                    .ForEach(b => TShock.Bans.RemoveBan(b.Key, true));
            Utils.Notice("成功执行解禁操作", HandyControl.Data.InfoType.Success);
        }
        public static void Whisper(this PlayerInfo info, string text)
        {
            info.Player.LastWhisper = TSPlayer.Server;
            info.Player.SendMessage($"<私聊> {text}", Microsoft.Xna.Framework.Color.Aqua);
            Utils.Notice("成功发送私聊 " + text, HandyControl.Data.InfoType.Success);
        }
        public static void Command(this PlayerInfo info, string text)
        {
            text ??= "help";
            if (Commands.HandleCommand(info.Player, text.StartsWith(Commands.Specifier) ? text : "/" + text))
                Utils.Notice("成功执行命令 " + text, HandyControl.Data.InfoType.Success);
            else
                Utils.Notice("执行失败", HandyControl.Data.InfoType.Warning);
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
            if (info.Account is not null)
            {
                if (info.Online)
                    info.Player.SendInfoMessage("你的账号已被移除");
                HookManager.OnAccountDelete(new(info.Account));
            }
            else
                Utils.Notice("当前无法删除此账号, 未能找到对应账号 (如注册后尚未登录", HandyControl.Data.InfoType.Warning);
        }
        #endregion
    }
}
