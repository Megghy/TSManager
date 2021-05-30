using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace TSManager.Modules
{
    class HookManager
    {
        public async static void OnPlayerJoin(JoinEventArgs args)
        {
            await Task.Run(() => {
                var plr = TShock.Players[args.Who];
                if (!Info.OnlinePlayers.Contains(plr)) TSMMain.GUIInvoke(() => Info.OnlinePlayers.Add(plr));
                if (plr.TryGetPlayerInfo(out var info))
                {
                    info.Player = plr;
                    info.Online = true;
                    TSMMain.GUIInvoke(() => { if (TSMMain.GUI.Bag_Tab.DataContext == info) BagManager.Refresh(false); });
                }
            });
        }
        public async static void OnPlayerLeave(LeaveEventArgs args)
        {
            await Task.Run(() => {
                var plr = TShock.Players[args.Who];
                TSMMain.GUIInvoke(() => Info.OnlinePlayers.Remove(plr));
                if (plr.TryGetPlayerInfo(out var info))
                {
                    info.Online = false;
                }
            });
        }
        public static void OnAccountCreate(TShockAPI.Hooks.AccountCreateEventArgs args)
        {
            TSMMain.GUIInvoke(() => Info.Players.Add(new(args.Account)));
            Utils.Notice($"发现新账号: {args.Account.Name}", HandyControl.Data.InfoType.Success);
        }
        public static void OnAccountDelete(TShockAPI.Hooks.AccountDeleteEventArgs args)
        {
            if (Utils.TryGetPlayerInfo(args.Account.Name, out var info))
            {
                TSMMain.GUIInvoke(() => {
                    Info.Players.Remove(Info.Players.SingleOrDefault(p => p.Name == args.Account.Name));
                    TSMMain.GUI.PlayerManage_MainTab.DataContext = null;
                    TSMMain.GUI.Bag_Tab.DataContext = null;
                    TSMMain.GUI.PlayerManage_Info.DataContext = null;
                    TSMMain.GUI.PlayerManage_Group.SelectedItem = -1;
                    //修改背包显示
                    BagManager.ClearAllItem();
                    TSMMain.GUI.Bag_ItemInfo.DataContext = null;
                    Utils.Notice($"已删除玩家 {info.Name}", HandyControl.Data.InfoType.Success);
                });
            }
        }
    }
}
