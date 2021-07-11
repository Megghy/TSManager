using System;
using System.Linq;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace TSManager.Modules
{
    class HookManager
    {
        internal static void RegisterHooks()
        {
            ServerApi.Hooks.ServerLeave.Register(TSMMain.Instance, OnPlayerLeave);
            ServerApi.Hooks.NetGreetPlayer.Register(TSMMain.Instance, OnPlayerJoin);

            GetDataHandlers.KillMe += OnPlayerDead;

            TShockAPI.Hooks.AccountHooks.AccountCreate += OnAccountCreate;
            TShockAPI.Hooks.AccountHooks.AccountDelete += OnAccountDelete;
        }
        public static void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            Task.Run(() =>
            {
                try {
                    if (TShock.Players[args.Who] is { } plr)
                    {
                        if (!Info.OnlinePlayers.Any(p => p.Name == plr.Name)) TSMMain.GUIInvoke(() => Info.OnlinePlayers.Add(plr));
                        if (plr.TryGetPlayerInfo(out var info))
                        {
                            info.Player = plr;
                            info.Online = true;
                            info.PlayTime = 0;
                            info.Account = TShock.UserAccounts.GetUserAccountByName(info.Name);
                            info.Data = TShock.CharacterDB.GetPlayerData(info.Player, (int)(info.Account?.ID));
                            TSMMain.GUIInvoke(() => { if (TSMMain.GUI.Bag_Tab.DataContext == info) BagManager.Refresh(false); });
                        }
                        ScriptManager.ExcuteScript(new(Data.ScriptData.Triggers.PlayerJoin, plr, ""));
                    }
                }
                catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
            });
        }
        public static void OnPlayerLeave(LeaveEventArgs args)
        {
            Task.Run(() =>
            {
                try {
                    if (TShock.Players[args.Who] is { } plr)
                    {
                        TSMMain.GUIInvoke(() => Info.OnlinePlayers.Remove(Info.OnlinePlayers.FirstOrDefault(p => p.Name == plr.Name)));
                        if (plr.TryGetPlayerInfo(out var info))
                        {
                            info.Online = false;
                        }
                        ScriptManager.ExcuteScript(new(Data.ScriptData.Triggers.PlayerLeave, plr, ""));
                    }
                }
                catch (Exception ex) { Utils.Notice(ex, HandyControl.Data.InfoType.Error); }
            });
        }
        public static void OnPlayerDead(object o, GetDataHandlers.KillMeEventArgs args)
        {
            ScriptManager.ExcuteScript(new(Data.ScriptData.Triggers.PlayerDead, args.Player, ""));
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
                TSMMain.GUIInvoke(() =>
                {
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
